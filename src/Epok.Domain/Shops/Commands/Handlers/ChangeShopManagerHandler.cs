using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Changes a manager of the specified shop.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if the user specified is already a shop manager.
    /// </exception>
    public class ChangeShopManagerHandler : ICommandHandler<ChangeShopManager>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeShopManagerHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeShopManager command)
        {
            var shop = await _repository.GetAsync<Shop>(command.ShopId);
            var newManager = await _repository.LoadAsync<User>(command.NewManagerId);
            var actingManager = shop.Manager;

            if (actingManager == newManager)
                throw new DomainException(SameManagerAssigned(newManager));

            shop.Manager = newManager;
            newManager.IsShopManager = true;
            actingManager.IsShopManager = false;

            await _eventTransmitter.BroadcastAsync(new ShopManagerChanged(actingManager, shop, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
