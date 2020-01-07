using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using System.Threading.Tasks;
using static Epok.Domain.Users.ExceptionCauses;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Revokes permission from a user on a CQRS resource.
    /// </summary>
    public class RevokePermissionHandler : ICommandHandler<RevokePermission>
    {
        private readonly IPermissionRepository _permissionRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public RevokePermissionHandler(IPermissionRepository permissionRepo,
            IEventTransmitter eventTransmitter)
        {
            _permissionRepo = permissionRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RevokePermission command)
        {
            var permission = await _permissionRepo.GetAsync(command.Id);
            if (permission.User.UserType == UserType.GlobalAdmin)
                throw new DomainException(RevokingGlobalAdminPermission(permission.User));

            await _permissionRepo.ArchiveAsync(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Permission>(permission, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
