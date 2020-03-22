using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Modifies a given user.
    /// </summary>
    public class ChangeUserDataHandler : ICommandHandler<ChangeUserData>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeUserDataHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeUserData command)
        {
            var user = await _repository.LoadAsync<User>(command.Id);

            if (user.FirstName == command.FirstName 
                && user.LastName == command.LastName 
                && user.Email == command.Email)
                return;

            user.Name = command.Name;
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.Email = command.Email;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(user, Trigger.Changed, command.InitiatorId));
        }
    }
}
