using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Shops.Commands;
using Epok.Domain.Shops.Commands.Handlers;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class ShopTests : SetupBase
    {
        [Test]
        public async Task CreateShop_Success()
        {
            //adhere
            var initialShops = ProductAssemblyShopCategory.Shops.DeepCopy();

            //arrange
            var command = new CreateShop
            {
                Name = "New shop name",
                ManagerId = GlobalAdmin.Id,
                ShopCategoryId = ProductAssemblyShopCategory.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateShopHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities<Shop>(EntityRepository, nameof(EntityRepository.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].Name, Is.EqualTo(command.Name));
            Assert.That(entities[0].Manager, Is.EqualTo(GlobalAdmin));
            Assert.That(entities[0].ShopCategory, Is.EqualTo(ProductAssemblyShopCategory));
            Assert.False(initialShops.Contains(entities[0]));
            Assert.True(ProductAssemblyShopCategory.Shops.Contains(entities[0]));

            var events = GetRecordedEvents<DomainEvent<Shop>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            //annul
            ProductAssemblyShopCategory.Shops = initialShops;
        }

        [Test]
        public void ChangeShopManager_FailFor_TheSameUserBeingAssigned()
        {
            //arrange
            var command = new ChangeShopManager
            {
                ShopId = ProductAssemblyShop.Id,
                NewManagerId = ManagerOfProductAssemblyShop.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ChangeShopManagerHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(SameManagerAssigned(ManagerOfProductAssemblyShop)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }

        [Test]
        public async Task ChangeShopManager_Success()
        {
            //adhere
            var initialManager = ProductAssemblyShop.Manager;

            //arrange
            var command = new ChangeShopManager
            {
                ShopId = ProductAssemblyShop.Id,
                NewManagerId = UserWithPermissions.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ChangeShopManagerHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(ProductAssemblyShop.Manager, Is.Not.EqualTo(initialManager));
            Assert.That(ProductAssemblyShop.Manager, Is.EqualTo(UserWithPermissions));

            var events = GetRecordedEvents<ShopManagerChanged>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Changed));
            Assert.That(events[0].Entity, Is.EqualTo(ProductAssemblyShop));
            Assert.That(events[0].Dismissed, Is.EqualTo(initialManager));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            //annul
            ProductAssemblyShop.Manager = initialManager;
        }

        [Test]
        public void ArchiveShop_FailFor_InventoryStillInStock()
        {
            //arrange
            var command = new ArchiveShop
            {
                Id = ProductAssemblyShop.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveShopHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingShopWithInventory(ProductAssemblyShop)));
            Assert.That(GetRecordedEvents<DomainEvent<Shop>>(), Is.Empty);
        }

        [Test]
        public async Task ArchiveShop_Success()
        {
            //arrange
            var command = new ArchiveShop
            {
                Id = ShopToRemove.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveShopHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var ids = GetRecordedIds(EntityRepository, nameof(EntityRepository.ArchiveAsync));
            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(command.Id));
            var events = GetRecordedEvents<DomainEvent<Shop>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].Entity, Is.EqualTo(ShopToRemove));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }
    }
}