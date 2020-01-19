using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Creates a new shop category.
    /// </summary>
    public class CreateShopCategoryHandler : ICommandHandler<CreateShopCategory>
    {
        private readonly IRepository<ShopCategory> _shopCategoryRepo;
        private readonly IArticleRepository _articleRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateShopCategoryHandler(IRepository<ShopCategory> shopCategoryRepo,
            IArticleRepository articleRepo, IEventTransmitter eventTransmitter)
        {
            _shopCategoryRepo = shopCategoryRepo;
            _articleRepo = articleRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateShopCategory command)
        {
            var shopCategory = new ShopCategory(command.Id, command.Name)
            {
                ShopType = command.ShopType
            };
            var articles = await _articleRepo.GetSomeAsync(command.Articles);
            foreach (var article in articles)
                shopCategory.Articles.Add(article);

            await _shopCategoryRepo.AddAsync(shopCategory);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Added,
                command.InitiatorId));
        }
    }
}
