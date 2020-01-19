using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;
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
        private readonly IRepository<User> _userRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveUserHandler(IRepository<User> userRepo, IEventTransmitter eventTransmitter)

        {
            _userRepo = userRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveUser command)
        {
            var user = await _userRepo.LoadAsync(command.Id);
            if (user.IsShopManager)
                throw new DomainException(ArchivingShopManager(user));

            await _userRepo.ArchiveAsync(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(user, Trigger.Removed, command.InitiatorId));
        }
    }
}
