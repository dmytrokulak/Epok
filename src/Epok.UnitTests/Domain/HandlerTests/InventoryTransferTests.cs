using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Commands.Handlers;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Shops.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class InventoryTransferTests : SetupBase
    {
        private TransferInventoryHandler _transferHandler;
        private const decimal AmountToHandle = 5;
        private const decimal AmountTooMuchToHandle = 15;

        [SetUp]
        public void SetUp()
        {
            _transferHandler = new TransferInventoryHandler(InventoryService, ReadOnlyRepo, EventTransmitter);
        }

        [Test]
        public async Task TransferInventory_Success()
        {
            //adhere
            var initialSourceInventory = MaterialStockpileShop.Inventory.DeepCopy();
            var initialTargetInventory = TimberComponentShop.Inventory.DeepCopy();

            //arrange
            var command = new TransferInventory
            {
                ArticleId = Material1Timber.Id,
                Amount = AmountToHandle,
                SourceShopId = MaterialStockpileShop.Id,
                TargetShopId = TimberComponentShop.Id,
                InitiatorId = GlobalAdmin.Id
            };

            //act
            await _transferHandler.HandleAsync(command);

            //assert
            var sourceDiff = initialSourceInventory.Of(Material1Timber).Amount
                             - MaterialStockpileShop.Inventory.Of(Material1Timber).Amount;
            Assert.That(sourceDiff, Is.EqualTo(command.Amount));
            var targetDiff = TimberComponentShop.Inventory.Of(Material1Timber).Amount
                             - initialTargetInventory.Of(Material1Timber).Amount;
            Assert.That(targetDiff, Is.EqualTo(command.Amount));
            Assert.That(sourceDiff, Is.EqualTo(targetDiff));

            Assert.That(GetRecordedEvents<InventoryTransferred>().Count, Is.EqualTo(1));
            Assert.That(GetRecordedEvents<ShopInventoryChanged>().Count, Is.EqualTo(2));

            //annul
            MaterialStockpileShop.Inventory = initialSourceInventory;
            TimberComponentShop.Inventory = initialTargetInventory;
        }

        [Test]
        public void TransferInventory_FailFor_LackOfInventory()
        {
            //arrange
            var command = new TransferInventory
            {
                ArticleId = Product1InteriorDoor.Id,
                Amount = AmountTooMuchToHandle,
                SourceShopId = ProductAssemblyShop.Id,
                TargetShopId = ProductStockpileShop.Id,
                InitiatorId = GlobalAdmin.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _transferHandler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotInStock(Product1InteriorDoor, ProductAssemblyShop)));
            Assert.That(GetRecordedEvents<InventoryTransferred>(), Is.Empty);
            Assert.That(GetRecordedEvents<ShopInventoryChanged>(), Is.Empty);
        }

        [Test]
        public void TransferInventory_FailFor_ArticleNotRegisteredWithReceivingWorkshop()
        {
            //arrange
            var command = new TransferInventory
            {
                ArticleId = Product1InteriorDoor.Id,
                Amount = AmountToHandle,
                SourceShopId = ProductAssemblyShop.Id,
                TargetShopId = MdfComponentShop.Id,
                InitiatorId = GlobalAdmin.Id
            };

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await _transferHandler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotAllowedInShop(Product1InteriorDoor, MdfComponentShop)));
            Assert.That(GetRecordedEvents<InventoryTransferred>(), Is.Empty);
            Assert.That(GetRecordedEvents<ShopInventoryChanged>(), Is.Empty);
        }
    }
}
