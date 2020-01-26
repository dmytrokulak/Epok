using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Tests.Setup;
using Epok.Domain.Users.Commands;
using Epok.Domain.Users.Commands.Handlers;
using Epok.Domain.Users.Entities;
using NUnit.Framework;
using System.Threading.Tasks;
using static Epok.Domain.Users.ExceptionCauses;

namespace Epok.Domain.Tests.HandlerTests
{
    [TestFixture]
    public class UserTests : SetupBase
    {
        [Test]
        public async Task CreateUser_Success()
        {
            //arrange
            var command = new CreateUser
            {
                Name = "User FirstName LastName",
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateUserHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities<User>(EntityRepository, nameof(EntityRepository.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].Name, Is.EqualTo(command.Name));
            Assert.That(entities[0].IsShopManager, Is.False);
            Assert.That(entities[0].Shop, Is.Null);

            var events = GetRecordedEvents<DomainEvent<User>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public async Task ArchiveUser_Success()
        {
            //arrange
            var command = new ArchiveUser
            {
                Id = UserToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveUserHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var ids = GetRecordedIds(EntityRepository, nameof(EntityRepository.ArchiveAsync));
            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(command.Id));

            var events = GetRecordedEvents<DomainEvent<User>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].Entity, Is.EqualTo(UserToArchive));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public void ArchiveUser_FailFor_UserBeingAssignedAsShopManager()
        {
            //arrange
            var command = new ArchiveUser
            {
                Id = ManagerOfProductAssemblyShop.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveUserHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingShopManager(ManagerOfProductAssemblyShop)));
            Assert.That(GetRecordedEvents<DomainEvent<User>>(), Is.Empty);
        }
    }
}
