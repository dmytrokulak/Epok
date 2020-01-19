using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Repositories;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Creates new shop and assigns a manager.
    /// </summary>
    public class CreateShopHandler : ICommandHandler<CreateShop>
    {
        private readonly IShopRepository _shopRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<ShopCategory> _shopCategoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateShopHandler(IShopRepository shopRepo, IRepository<User> userRepo,
            IRepository<ShopCategory> shopCategoryRepo,
            IEventTransmitter eventTransmitter)
        {
            _shopRepo = shopRepo;
            _userRepo = userRepo;
            _shopCategoryRepo = shopCategoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateShop command)
        {
            var shopCategory = await _shopCategoryRepo.LoadAsync(command.ShopCategoryId);

            var manager = await _userRepo.LoadAsync(command.ManagerId);
            if (manager.Shop != null)
                throw new DomainException(UserIsAlreadyManager(manager));

            var shop = new Shop(command.Id, command.Name)
            {
                ShopCategory = shopCategory,
                Manager = manager,
                IsEntryPoint = command.IsEntryPoint,
                IsExitPoint = command.IsExitPoint
            };

            shopCategory.Shops.Add(shop);
            await _shopRepo.AddAsync(shop);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<User>(manager, Trigger.Changed,
                command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Shop>(shop, Trigger.Added, command.InitiatorId));
        }
    }
}
