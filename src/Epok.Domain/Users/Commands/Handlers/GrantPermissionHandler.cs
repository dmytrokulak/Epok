using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using System;
using System.Threading.Tasks;
using static Epok.Domain.Users.ExceptionCauses;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Grant permission to a user on a CQRS resource.
    /// </summary>
    public class GrantPermissionHandler : ICommandHandler<GrantPermission>
    {
        private readonly IPermissionRepository _permissionRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<CqrsResource> _resourceRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public GrantPermissionHandler(IPermissionRepository permissionRepo, IRepository<User> userRepo,
            IRepository<CqrsResource> resourceRepo, IEventTransmitter eventTransmitter)

        {
            _userRepo = userRepo;
            _resourceRepo = resourceRepo;
            _permissionRepo = permissionRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(GrantPermission command)
        {
            var user = await _userRepo.LoadAsync(command.UserId);
            var handler = await _resourceRepo.LoadAsync(command.HandleId);

            var permission = await _permissionRepo.Find(user, handler);
            if (permission != null)
                throw new DomainException(DuplicatingGrant(permission));

            permission = new Permission(Guid.NewGuid(), $"Grant on {handler.Name} for {user.Name}")
            {
                User = user,
                Handler = handler,
                Restriction = command.Restriction,
            };

            await _permissionRepo.AddAsync(permission);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Permission>(permission, Trigger.Added,
                command.InitiatorId));
        }
    }
}
