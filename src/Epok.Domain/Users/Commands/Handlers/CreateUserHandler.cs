using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Users.Commands.Handlers
{
    /// <summary>
    /// Creates a user of the system.
    /// </summary>
    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateUserHandler(IRepository<User> userRepo, IEventTransmitter eventTransmitter)

        {
            _userRepo = userRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateUser command)
        {
            var user = new User(command.Id, command.Name);

            await _userRepo.AddAsync(user);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(user, Trigger.Added, command.InitiatorId));
        }
    }
}
