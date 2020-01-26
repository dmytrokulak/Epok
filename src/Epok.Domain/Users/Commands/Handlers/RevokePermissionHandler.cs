using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using static Epok.Domain.Users.ExceptionCauses;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Revokes permission from a user on a domain resource.
    /// </summary>
    public class RevokePermissionHandler : ICommandHandler<RevokePermission>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public RevokePermissionHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RevokePermission command)
        {
            var permission = await _repository.GetAsync<Permission>(command.Id);
            if (permission.User.UserType == UserType.GlobalAdmin)
                throw new DomainException(RevokingGlobalAdminPermission(permission.User));

            await _repository.ArchiveAsync<Permission>(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Permission>(permission, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
