using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Creates a user of the system.
    /// </summary>
    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateUserHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateUser command)
        {
            var user = new User(command.Id, command.Name)
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                //UserType = command.UserType,
                Email = command.Email
            };

            await _repository.AddAsync(user);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(user, Trigger.Added, command.InitiatorId));
        }
    }
}
