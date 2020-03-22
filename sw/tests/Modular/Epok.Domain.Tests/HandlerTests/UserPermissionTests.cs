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
    public class UserPermissionTests : SetupBase
    {
        [Test]
        public async Task GrantPermission_Success()
        {
            //arrange
            var command = new GrantPermission
            {
                UserId = ManagerOfProductAssemblyShop.Id,
                ResourceId = ProduceInventoryItemHandler.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new GrantPermissionHandler(EntityRepository, PermissionRepo, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities<Permission>(PermissionRepo, nameof(PermissionRepo.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].Resource, Is.EqualTo(ProduceInventoryItemHandler));
            Assert.That(entities[0].User, Is.EqualTo(ManagerOfProductAssemblyShop));

            var events = GetRecordedEvents<DomainEvent<Permission>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public void GrantPermission_FailFor_PermissionAlreadyGranted()
        {
            //arrange
            var command = new GrantPermission
            {
                UserId = UserWithPermissions.Id,
                ResourceId = ProduceInventoryItemHandler.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new GrantPermissionHandler(EntityRepository, PermissionRepo, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(DuplicatingGrant(UserWithPermissionsOnProduceInventoryItemHandler)));
            Assert.That(GetRecordedEvents<DomainEvent<Permission>>(), Is.Empty);
        }

        [Test]
        public async Task RevokePermission_Success()
        {
            //arrange
            var command = new RevokePermission
            {
                Id = UserWithPermissionsOnProduceInventoryItemHandler.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new RevokePermissionHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var ids = GetRecordedIds(EntityRepository, nameof(EntityRepository.RemoveAsync));
            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(command.Id));

            var events = GetRecordedEvents<DomainEvent<Permission>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].Entity, Is.EqualTo(UserWithPermissionsOnProduceInventoryItemHandler));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Ignore("ToDo:4 review RevokePermission_FailFor_RevokingOnRevokePermissionHandler - questionable stuff")]
        public void RevokePermission_FailFor_RevokingOnRevokePermissionHandler()
        {
            //arrange
            var command = new RevokePermission
            {
                Id = GlobalAdminOnProduceInventoryItemHandler.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new RevokePermissionHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(RevokingGlobalAdminPermission(GlobalAdmin)));
            Assert.That(GetRecordedEvents<DomainEvent<Permission>>(), Is.Empty);
        }
    }
}
