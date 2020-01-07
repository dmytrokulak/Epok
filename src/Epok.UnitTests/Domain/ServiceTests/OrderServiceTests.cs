using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders;
using Epok.Domain.Orders.Entities;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using static Epok.Domain.Orders.ExceptionCauses;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.UnitTests.Domain.ServiceTests
{
    [TestFixture]
    public class OrderServiceTests : SetupBase
    {

        #region Assign and remove tests

        [Test]
        public void AssignOrder_Success()
        {
            //Arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Orders = new HashSet<Order>();
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, 5),
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                }
            };

            //Act
            OrderService.AssignOrder(order, shop);

            //Assert
            Assert.That(shop.Orders.Contains(order), Is.True);
            Assert.That(order.Shop, Is.EqualTo(shop));
        }

        [Test]
        public void AssignOrder_FailFor_ArticleNotAllowed()
        {
            //Arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Orders = new HashSet<Order>();
            shop.ShopCategory.Articles = new HashSet<Article>();
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, 5),
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                }
            };

            //Assert () => Act
            var ex = Assert.Throws<DomainException>(() => OrderService.AssignOrder(order, shop));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotAllowedInShop(Product1InteriorDoor, shop)));
        }

        [Test]
        public void RemoveOrder_Success()
        {
            //Arrange
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, 5),
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                }
            };
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Orders = new HashSet<Order>() {order};
            order.Shop = shop;

            //Act
            OrderService.RemoveOrder(order, shop);

            //Assert
            Assert.That(!shop.Orders.Contains(order), Is.True);
            Assert.That(order.Shop, Is.Null);
        }

        #endregion

        #region Shipment tests

        [TestCase(5, 5, true)]
        [TestCase(5, 4, false)]
        public void IsReadyForShipment(decimal ordered, decimal produced, bool expected)
        {
            //Arrange
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, ordered),
                    new InventoryItem(Product2InteriorDoor, 3)
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, produced),
                    new InventoryItem(Product2InteriorDoor, 3)
                }
            };

            //Act
            var isReady = OrderService.IsReadyForShipment(order);

            //Assert
            Assert.That(isReady, Is.EqualTo(expected));
        }

        [Test]
        public void ShipOrder_Success()
        {
            //Arrange
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, 5),
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                }
            };
            var shop = ProductStockpileShop.DeepCopy();
            shop.Orders = new HashSet<Order>() {order};
            order.Shop = shop;


            //Act
            OrderService.ShipOrder(order);

            //Assert
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Shipped));
            Assert.That(order.ShippedAt, Is.Not.Null);
        }

        [Test]
        public void ShipOrder_FailFor_OrderNotAtExitPoint()
        {
            //Arrange
            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>()
                {
                    new InventoryItem(Product1InteriorDoor, 5),
                },
                ItemsProduced = new HashSet<InventoryItem>()
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                }
            };
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Orders = new HashSet<Order>() {order};
            order.Shop = shop;

            //Assert () => Act
            var ex = Assert.Throws<DomainException>(() => OrderService.ShipOrder(order));
            Assert.That(ex.Message, Is.EqualTo(OrderNotAtExitPoint(order)));
        }

        #endregion

        #region SubOrders tests

        [Test]
        public void CalculateInventoryInput_Success()
        {
            //Arrange
            var itemP1 = new InventoryItem(Product1InteriorDoor, 20);
            var itemP2 = new InventoryItem(Product2InteriorDoor, 10);

            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem> {itemP1, itemP2},
                ItemsProduced = new HashSet<InventoryItem>
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                    InventoryItem.Empty(Product2InteriorDoor),
                },
                ShipmentDeadline = new DateTimeOffset(2020, 1, 1, 1, 1, 1, TimeSpan.Zero)
            };

            var inventory = OrderService.CalculateInventoryInput(order).ToList();

            Assert.That(inventory.Count(), Is.EqualTo(inventory.Distinct().Count()));
            Assert.That(inventory.Of(Product1InteriorDoor).Amount, Is.EqualTo(20));
            Assert.That(inventory.Of(Product2InteriorDoor).Amount, Is.EqualTo(10));
            Assert.That(inventory.Of(Component1Vertical).Amount, Is.EqualTo(60));
            Assert.That(inventory.Of(Component2Horizontal).Amount, Is.EqualTo(60));
            Assert.That(inventory.Of(Component3MdfFiller).Amount, Is.EqualTo(50));
            Assert.That(inventory.Of(Component4GlassFiller).Amount, Is.EqualTo(10));
        }

        [Test]
        public void CreateSubOrder_Success()
        {
            var item = new InventoryItem(Component1Vertical, 60);

            var order = new Order(Guid.NewGuid(), "Test order")
            {
                ItemsOrdered = new HashSet<InventoryItem>
                {
                    new InventoryItem(Product1InteriorDoor, 20),
                    new InventoryItem(Product2InteriorDoor, 10),
                },
                ItemsProduced = new HashSet<InventoryItem>
                {
                    InventoryItem.Empty(Product1InteriorDoor),
                    InventoryItem.Empty(Product2InteriorDoor),
                },
                ShipmentDeadline = new DateTimeOffset(2020, 1, 1, 1, 1, 1, TimeSpan.Zero)
            };
            var now = DateTimeOffset.Now;
            var subOrder = OrderService.CreateSubOrder(item, order);

            Assert.That(subOrder.ParentOrder, Is.EqualTo(order));
            Assert.That(subOrder.ItemsOrdered.Contains(item), Is.True);
            Assert.That(subOrder.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(subOrder.Type, Is.EqualTo(OrderType.Internal));
            Assert.That(order.SubOrders.Contains(subOrder), Is.True);
            Assert.That(TimberComponentShop.Orders.Contains(subOrder), Is.True);
            Assert.That(subOrder.CreatedAt, Is.AtLeast(now));
            Assert.That(subOrder.CreatedAt, Is.AtMost(DateTimeOffset.Now));
        }

        #endregion

        #region Increase Produced Tests

        [Test]
        public void IncreaseProduced_Success()
        {
            //arrange
            var order = Product1Order.DeepCopy();
            var item = new InventoryItem(Product1InteriorDoor, 10);

            //act
            OrderService.IncreaseProduced(item, order);

            //assert
            var diff = order.ItemsProduced.Of(Product1InteriorDoor).Amount
                       - Product1Order.ItemsProduced.Of(Product1InteriorDoor).Amount;
            Assert.That(diff, Is.EqualTo(item.Amount));
        }

        [Test]
        public void IncreaseProduced_FailFor_ArticleNotInOrder()
        {
            //arrange
            var order = Product1Order.DeepCopy();
            var item = new InventoryItem(Product2InteriorDoor, 10);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => OrderService.IncreaseProduced(item, order));
            Assert.That(ex.Message, Is.EqualTo(OrderNotContainArticle(order, item.Article)));
        }

        [Test]
        public void IncreaseProduced_FailFor_OrderOverflow()
        {
            //arrange
            var order = Product1Order.DeepCopy();
            var item = new InventoryItem(Product1InteriorDoor, 30);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => OrderService.IncreaseProduced(item, order));
            Assert.That(ex.Message, Is.EqualTo(OrderOverflow(order, Product1InteriorDoor, item.Amount)));
        }

        #endregion
    }
}
