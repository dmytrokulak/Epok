using Epok.Core.Utilities;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Commands.Handlers;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Shops.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class InventorySpoilageTests : SetupBase
    {
        [Test]
        public async Task ReportSpoilage_Success()
        {
            //adhere
            var initial = ProductAssemblyShop.Inventory.DeepCopy();

            //arrange
            var command = new ReportSpoilage
            {
                Amount = 1,
                ArticleId = Product1InteriorDoor.Id,
                OrderId = Product1Order.Id,
                Reason = "Something went wrong",
                Fixable = true,
                ShopId = ProductAssemblyShop.Id,
                InitiatorId = GlobalAdmin.Id
            };

            var handler = new ReportSpoilageHandler(EntityRepository, InventoryService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.AddAsync)), Is.EqualTo(1));
            var entity = GetRecordedEntities<SpoilageReport>(EntityRepository, nameof(EntityRepository.AddAsync)).Single();
            Assert.That(entity.Item.Article, Is.EqualTo(Product1InteriorDoorSpoiled));
            Assert.That(((SpoiledArticle) entity.Item.Article).Fixable, Is.EqualTo(command.Fixable));
            Assert.That(entity.Item.Amount, Is.EqualTo(command.Amount));
            Assert.That(entity.Reason, Is.EqualTo(command.Reason));

            Assert.That(GetRecordedEvents<ShopInventoryChanged>().Count, Is.EqualTo(1));
            Assert.That(GetRecordedEvents<SpoilageReported>().Count, Is.EqualTo(1));

            //annul
            ProductAssemblyShop.Inventory = initial;
        }
    }
}
