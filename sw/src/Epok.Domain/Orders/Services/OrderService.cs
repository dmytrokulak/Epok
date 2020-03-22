using Epok.Core.Domain.Exceptions;
using Epok.Core.Providers;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using static Epok.Domain.Inventory.ExceptionCauses;
using static Epok.Domain.Orders.ExceptionCauses;

namespace Epok.Domain.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly ITimeProvider _timeProvider;

        public OrderService(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public Order CreateSubOrder(InventoryItem item, Order parent)
        {
            if (item.Article.ProductionShopCategory == null)
                throw new DomainException(ArticleNotProducible(item.Article));

            var subOrder = new Order(Guid.NewGuid(), $"{item.Article.Name} suborder of {parent}")
            {
                ReferenceOrder = parent?.ReferenceOrder ?? parent,
                ParentOrder = parent,
                ItemsOrdered = item.Collect().ToHashSet(),
                ItemsProduced = InventoryItem.Empty(item.Article).Collect().ToHashSet(),
                Status = OrderStatus.New,
                Type = OrderType.Internal,
                Customer = parent.Customer,
                CreatedAt = DateTimeOffset.Now,
                ShipmentDeadline = _timeProvider.Subtract(parent.ShipmentDeadline, item.TimeToProduce)
            };

            parent.SubOrders.Add(subOrder);
            parent.ReferenceOrder?.SubOrders.Add(subOrder);

            var shop = item.Article.ProductionShopCategory?.Shops
                           .OrderBy(s => s.Orders.Min(o => o?.EstimatedCompletionAt)).FirstOrDefault() ??
                       item.Article?.ProductionShopCategory?.DefaultShop;

            if (shop != null)
                AssignOrder(subOrder, shop);

            return subOrder;
        }

        public IEnumerable<InventoryItem> CalculateInventoryInput(Order order)
        {
            Guard.Against.Null(order, nameof(order));

            var inventory = order.ItemsOrdered
                .SelectMany(CalculateInventories)
                .Concat(order.ItemsOrdered)
                .GroupBy(i => i.Article)
                .Select(g => new InventoryItem(g.Key, g.Sum(i => i.Amount)));

            return inventory;

            IEnumerable<InventoryItem> CalculateInventories(InventoryItem subItem)
            {
                var subInventory = new List<InventoryItem>();
                foreach (var input in subItem.Article.PrimaryBillOfMaterial.Input)
                {
                    subInventory.Add(new InventoryItem(input.Article, input.Amount * subItem.Amount));
                    if (input.Article.PrimaryBillOfMaterial != null)
                        subInventory.AddRange(CalculateInventories(input));
                }

                return subInventory;
            }
        }

        public void ShipOrder(Order order)
        {
            if (!order.Shop.IsExitPoint)
                throw new DomainException(OrderNotAtExitPoint(order));
            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTimeOffset.Now;
        }

        public void AssignOrder(Order order, Shop shop)
        {
            foreach (var item in order.ItemsOrdered)
                if (!shop.ShopCategory.Articles.Contains(item.Article))
                    throw new DomainException(ArticleNotAllowedInShop(item.Article, shop));

            shop.Orders.Add(order);
            order.Shop = shop;
        }

        public void RemoveOrder(Order order, Shop shop)
        {
            shop.Orders.Remove(order);
            order.Shop = null;
        }

        public void IncreaseProduced(InventoryItem item, Order order)
        {
            var ordered = order.ItemsOrdered.Of(item.Article);
            if (ordered is null)
                throw new DomainException(OrderNotContainArticle(order, item.Article));

            var produced = order.ItemsProduced.Of(item.Article);
            if (item.Amount > (ordered.Amount - produced.Amount))
                throw new DomainException(OrderOverflow(order, item.Article, item.Amount));

            var newAmount = produced.Amount + item.Amount;
            if (newAmount > ordered.Amount)
                throw new DomainException(OrderOverflow(order, item.Article, item.Amount));
            produced.Amount = newAmount;

            if (IsReadyForShipment(order))
                order.Status = OrderStatus.ReadyForShipment;
        }

        public bool IsReadyForShipment(Order order)
            => order.ItemsOrdered.All(i => order.ItemsProduced.Of(i.Article).Amount == i.Amount);
    }
}
