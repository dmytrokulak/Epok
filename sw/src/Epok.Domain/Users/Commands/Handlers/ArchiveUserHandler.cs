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
    /// Archives a user.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if user is a shop manager. 
    /// </exception>
    public class ArchiveUserHandler : ICommandHandler<ArchiveUser>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveUserHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveUser command)
        {
            var user = await _repository.LoadAsync<User>(command.Id);
            if (user.IsShopManager)
                throw new DomainException(ArchivingShopManager(user));

            await _repository.RemoveAsync(user);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(user, Trigger.Removed, command.InitiatorId));
        }
    }
}
