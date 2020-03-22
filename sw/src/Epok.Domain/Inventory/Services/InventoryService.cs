using Epok.Core.Domain.Exceptions;
using Epok.Core.Providers;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Inventory.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IArticleRepository _articleRepo;
        private readonly ITimeProvider _timeProvider;

        public InventoryService(IInventoryRepository inventoryRepo, IArticleRepository articleRepo, 
            ITimeProvider timeProvider)
        {
            _inventoryRepo = inventoryRepo;
            _articleRepo = articleRepo;
            _timeProvider = timeProvider;
        }

        public InventoryItem Produce(Shop shop, InventoryItem item, Order order)
        {
            Guard.Against.Null(shop, nameof(shop));
            Guard.Against.Null(item, nameof(item));
            Guard.Against.Null(order, nameof(order));
            Guard.Against.Zero(item.Amount, nameof(item.Amount));

            var article = item.Article;
            var amount = item.Amount;

            if (!shop.ShopCategory.Articles.Contains(article))
                throw new DomainException(ArticleNotAllowedInShop(article, shop));

            var orderItem = order.ItemsOrdered.Of(article);
            if (orderItem == null)
                throw new DomainException(ArticleNotInOrder(order, article));

            var amountYetRequired =
                orderItem.Amount - order.ItemsProduced.Of(article).Amount;

            if (amount > amountYetRequired)
                throw new DomainException(
                    ProductionRequestExceedsRequirements(order, amountYetRequired, article, amount));

            DecreaseArticleInputs(item, shop);
            var output = new InventoryItem(article, amount);
            IncreaseInventory(output, shop);

            return output;
        }

        public async Task<InventoryItem> Produce(Shop shop, Guid articleId, decimal amount, Order order)
        {
            var article = await _articleRepo.GetAsync(articleId);
            return Produce(shop, new InventoryItem(article, amount), order);
        }

        public InventoryItem Transfer(Shop source, Shop target, InventoryItem item)
        {
            Guard.Against.Null(source, nameof(source));
            Guard.Against.Null(target, nameof(target));
            Guard.Against.Null(item, nameof(item));
            Guard.Against.Zero(item.Amount, nameof(item.Amount));

            var article = item.Article;
            var amount = item.Amount;

            if (!target.ShopCategory.Articles.Contains(article))
                throw new DomainException(ArticleNotAllowedInShop(article, target));

            var sourceAmount = source.Inventory.Of(article)?.Amount;
            if (sourceAmount == null)
                throw new DomainException(ArticleNotInStock(article, source));
            if (sourceAmount < amount)
                throw new DomainException(InsufficientInventory(article, amount, sourceAmount.Value));

            var transferred = new InventoryItem(article, amount);
            DecreaseInventory(transferred, source);
            IncreaseInventory(transferred, target);

            return transferred;
        }

        public InventoryItem ReportSpoilage(InventoryItem item, Order order, Shop shop)
        {
            Guard.Against.Null(shop, nameof(shop));
            Guard.Against.Null(order, nameof(order));
            Guard.Against.Null(item, nameof(item));

            var spoiled = (SpoiledArticle) item.Article;
            var amount = item.Amount;

            if (spoiled.ProductionShopCategory == shop.ShopCategory)
                DecreaseArticleInputs(new InventoryItem(spoiled.Article, amount), shop);
            else
                DecreaseInventory(new InventoryItem(spoiled.Article, amount), shop);

            var spoilage = new InventoryItem(spoiled, amount);
            IncreaseInventory(spoilage, shop);

            return spoilage;
        }

        public async Task<InventoryItem> ReportSpoilage(Guid articleId, decimal amount, bool fixable, Order order,
            Shop shop)
        {
            var article = await _articleRepo.LoadAsync(articleId);
            var spoiled = await _articleRepo.FindSpoiledCounterpartAsync(article, fixable);
            return ReportSpoilage(new InventoryItem(spoiled, amount), order, shop);
        }

        private void DecreaseArticleInputs(InventoryItem item, Shop shop)
        {
            foreach (var subItem in item.Article.PrimaryBillOfMaterial.Input)
            {
                var stock = shop.Inventory.Of(subItem.Article);
                if (stock == null)
                    throw new DomainException(ArticleNotInStock(subItem.Article, shop));
                var required = subItem.Amount * item.Amount;
                if (stock.Amount < required)
                    throw new DomainException(InsufficientInventory(subItem.Article, required, stock.Amount));
                DecreaseInventory(new InventoryItem(subItem.Article, subItem.Amount * item.Amount), shop);
            }
        }

        public void IncreaseInventory(InventoryItem item, Shop shop)
        {
            Guard.Against.Null(shop, nameof(shop));
            Guard.Against.Null(item, nameof(item));

            if (!shop.ShopCategory.Articles.Contains(item.Article))
                throw new DomainException(ArticleNotAllowedInShop(item.Article, shop));

            var stock = shop.Inventory.Of(item.Article);
            if (stock == null)
                shop.Inventory.Add(item);
            else
                stock.Amount += item.Amount;
        }

        public void IncreaseInventory(IEnumerable<InventoryItem> items, Shop shop)
        {
            Guard.Against.Null(items, nameof(items));
            foreach (var item in items)
                IncreaseInventory(item, shop);
        }

        public void DecreaseInventory(InventoryItem item, Shop shop)
        {
            Guard.Against.Null(shop, nameof(shop));
            Guard.Against.Null(item, nameof(item));

            var stock = shop.Inventory.Of(item.Article);
            if (stock == null)
                throw new DomainException(ArticleNotInStock(item.Article, shop));

            var newAmount = stock.Amount - item.Amount;
            if (newAmount < 0)
                throw new DomainException(InsufficientInventory(item.Article,
                    item.Amount, stock.Amount));

            stock.Amount = newAmount;
        }

        public void DecreaseInventory(IEnumerable<InventoryItem> items, Shop shop)
        {
            Guard.Against.Null(items, nameof(items));
            foreach (var item in items)
                DecreaseInventory(item, shop);
        }

        public async Task<DateTimeOffset> CalculateTimeOfCompletion(IEnumerable<InventoryItem> items)
        {
            DateTimeOffset estimatedCompletionAt = DateTimeOffset.Now;
            foreach (var item in items)
            {
                var eta = await CalculateTimeToCompletion(item, DateTimeOffset.Now);

                if (eta > estimatedCompletionAt)
                    estimatedCompletionAt = eta;
            }

            return estimatedCompletionAt;
        }

        private async Task<DateTimeOffset> CalculateTimeToCompletion(InventoryItem item, DateTimeOffset eta)
        {
            if (item.Article.ProductionShopCategory == null)
                return eta;

            var amountInStock = await _inventoryRepo.FindSpareInventoryAsync(item.Article);
            if (amountInStock == item.Amount)
                return eta;

            var subItem = new InventoryItem(item.Article, item.Amount - amountInStock);
            var componentEta = _timeProvider.Add(subItem.EarliestProductionStartTime, item.TimeToProduce);
            //ToDo:3 check if CalculateTimeToCompletionAsync returns the max level as sum of all iterations
            //or only takes into account the longest timespan at second step
            return item.Article.PrimaryBillOfMaterial.Input
                .Select(async i => await CalculateTimeToCompletion(i, componentEta))
                .Max(t => t.Result);
        }

        public async Task<decimal> CalculateAllocatableAmount(Article article)
        {
            //find spare amount first
            var result = await _inventoryRepo.FindSpareInventoryAsync(article);
            if (article.PrimaryBillOfMaterial == null)
                return result;

            //if article can be manufactured - find possible amount to manufacture
            var producibleAmounts = new List<decimal>();
            foreach (var item in article.PrimaryBillOfMaterial.Input)
            {
                var subResult = await CalculateAllocatableAmount(item.Article);
                producibleAmounts.Add(subResult * item.Amount);
            }

            //calculations have been done by single input so take the least result
            result += producibleAmounts.Min();
            return result;
        }
    }
}
