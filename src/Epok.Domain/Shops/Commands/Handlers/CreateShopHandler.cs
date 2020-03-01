using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Creates new shop and assigns a manager.
    /// </summary>
    public class CreateShopHandler : ICommandHandler<CreateShop>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateShopHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateShop command)
        {
            var shopCategory = await _repository.LoadAsync<ShopCategory>(command.ShopCategoryId);

            var manager = await _repository.LoadAsync<User>(command.ManagerId);
            if (manager.Shop != null)
                throw new DomainException(UserIsAlreadyManager(manager));
            manager.IsShopManager = true;

            var shop = new Shop(command.Id, command.Name)
            {
                ShopCategory = shopCategory,
                Manager = manager,
                IsEntryPoint = command.IsEntryPoint,
                IsExitPoint = command.IsExitPoint
            };

            if (shopCategory.Shops.Count == 0)
                shop.IsDefaultForCategory = true;

            shopCategory.Shops.Add(shop);
            await _repository.AddAsync(shop);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(manager, Trigger.Changed,
                command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Shop>(shop, Trigger.Added, command.InitiatorId));
        }
    }
}
