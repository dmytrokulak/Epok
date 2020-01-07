using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Creates a new shop category.
    /// </summary>
    public class CreateShopCategoryHandler : ICommandHandler<CreateShopCategory>
    {
        private readonly IRepository<ShopCategory> _shopCategoryRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateShopCategoryHandler(IRepository<ShopCategory> shopCategoryRepo,
            IInventoryRepository inventoryRepo, IEventTransmitter eventTransmitter)
        {
            _shopCategoryRepo = shopCategoryRepo;
            _inventoryRepo = inventoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateShopCategory command)
        {
            var shopCategory = new ShopCategory(command.Id, command.Name)
            {
                ShopType = command.ShopType
            };
            var articles = await _inventoryRepo.GetSomeAsync(command.Articles);
            foreach (var article in articles)
                shopCategory.Articles.Add(article);

            await _shopCategoryRepo.AddAsync(shopCategory);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Added,
                command.InitiatorId));
        }
    }
}
