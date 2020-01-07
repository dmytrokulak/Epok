using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Commands.Handlers;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class InventoryProduceTests : SetupBase
    {
        private ProduceInventoryItemHandler _produceHandler;
        private const decimal AmountToHandle = 5;
        private const decimal AmountTooMuchToHandle = 15;

        [SetUp]
        public void SetUp()
        {
            _produceHandler =
                new ProduceInventoryItemHandler(ReadOnlyRepo, InventoryService, OrderService, EventTransmitter);
        }

        [Test]
        public async Task ProduceInventoryItem_Success()
        {
            //adhere
            var shopInitialInventory = ProductAssemblyShop.Inventory.DeepCopy();
            var orderInitialProduced = Product1Order.ItemsProduced.DeepCopy();

            //arrange
            var command = new ProduceInventoryItem
            {
                ShopId = ProductAssemblyShop.Id,
                ArticleId = Product1InteriorDoor.Id,
                Amount = AmountToHandle,
                OrderId = Product1Order.Id,
                InitiatorId = GlobalAdmin.Id
            };

            //act
            await _produceHandler.HandleAsync(command);

            //assert
            var shopChange = ProductAssemblyShop.Inventory.Of(Product1InteriorDoor).Amount -
                             (shopInitialInventory.Of(Product1InteriorDoor)?.Amount ?? 0);
            Assert.That(shopChange, Is.EqualTo(AmountToHandle));

            var orderChanged = Product1Order.ItemsProduced.Of(Product1InteriorDoor).Amount -
                               orderInitialProduced.Of(Product1InteriorDoor).Amount;
            Assert.That(orderChanged, Is.EqualTo(AmountToHandle));

            var events = GetRecordedEvents<DomainEvent<Shop>>();
            Assert.That(events.Count, Is.EqualTo(2));
            Assert.That(events.OfType<ShopInventoryChanged>().Count(), Is.EqualTo(1));
            Assert.That(events.OfType<InventoryItemProduced>().Count(), Is.EqualTo(1));

            //annul
            ProductAssemblyShop.Inventory = shopInitialInventory;
            Product1Order.ItemsProduced = orderInitialProduced;
        }

        [Test]
        public void ProduceInventoryItem_FailFor_LackOfMaterials()
        {
            //arrange
            var command = new ProduceInventoryItem
            {
                ShopId = ProductAssemblyShop.Id,
                ArticleId = Product1InteriorDoor.Id,
                Amount = AmountTooMuchToHandle,
                OrderId = Product1Order.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _produceHandler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(InsufficientInventory(Component1Vertical, 30, 10)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }

        [Test]
        public void ProduceInventoryItem_FailFor_ProductNotRegistered()
        {
            //arrange
            var command = new ProduceInventoryItem
            {
                ShopId = ProductAssemblyShop.Id,
                ArticleId = Material1Timber.Id,
                Amount = AmountToHandle,
                OrderId = Product1Order.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _produceHandler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotAllowedInShop(Material1Timber, ProductAssemblyShop)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }

        [Test]
        public void ProduceInventoryItem_FailFor_OrderNotHavingProductIncluded()
        {
            //arrange
            var command = new ProduceInventoryItem
            {
                ShopId = ProductAssemblyShop.Id,
                ArticleId = Product2InteriorDoor.Id,
                Amount = AmountToHandle,
                OrderId = Product1Order.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _produceHandler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotInOrder(Product1Order, Product2InteriorDoor)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }

        [Test]
        public void ProduceInventoryItem_FailFor_OrderAmountToProduceBeingLessThanAmountSubmittedForProduction()
        {
            //arrange
            var amountToComplete = Product1Order.ItemsOrdered.Of(Product1InteriorDoor).Amount
                                   - Product1Order.ItemsProduced.Of(Product1InteriorDoor).Amount;
            var amountSubmitted = amountToComplete * 2;

            var command = new ProduceInventoryItem
            {
                ShopId = ProductAssemblyShop.Id,
                ArticleId = Product1InteriorDoor.Id,
                Amount = amountSubmitted,
                OrderId = Product1Order.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _produceHandler.HandleAsync(command));
            Assert.That(ex.Message,
                Is.EqualTo(ProductionRequestExceedsRequirements(Product1Order, 20, Product1InteriorDoor,
                    amountSubmitted)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }
    }
}
