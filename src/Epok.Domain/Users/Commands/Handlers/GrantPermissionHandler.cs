using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using System;
using System.Threading.Tasks;
using static Epok.Domain.Users.ExceptionCauses;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Grant permission to a user on a domain resource.
    /// </summary>
    public class GrantPermissionHandler : ICommandHandler<GrantPermission>
    {
        private readonly IPermissionRepository _permissionRepo;
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public GrantPermissionHandler(IEntityRepository repository, IPermissionRepository permissionRepo,
            IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _permissionRepo = permissionRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(GrantPermission command)
        {
            var user = await _repository.LoadAsync<User>(command.UserId);
            var resource = await _repository.LoadAsync<DomainResource>(command.ResourceId);

            var permission = await _permissionRepo.Find(user, resource);
            if (permission != null)
                throw new DomainException(DuplicatingGrant(permission));

            permission = new Permission(Guid.NewGuid(), $"Grant on {resource.Name} for {user.Name}")
            {
                User = user,
                Resource = resource,
                //ToDo:4 Restriction = command.Restriction,
            };

            await _permissionRepo.AddAsync(permission);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Permission>(permission, Trigger.Added,
                command.InitiatorId));
        }
    }
}
