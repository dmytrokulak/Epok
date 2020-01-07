using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Allows article to be stored
    /// in shops of the specified category.
    /// </summary>
    public class AllowArticleHandler : ICommandHandler<AllowArticle>
    {
        private readonly IRepository<ShopCategory> _categoryRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public AllowArticleHandler(IRepository<ShopCategory> categoryRepo, IInventoryRepository inventoryRepo,
            IEventTransmitter eventTransmitter)
        {
            _categoryRepo = categoryRepo;
            _inventoryRepo = inventoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AllowArticle command)
        {
            var shopCategory = await _categoryRepo.GetAsync(command.ShopCategoryId);
            var article = await _inventoryRepo.GetAsync(command.ArticleId);

            shopCategory.Articles.Add(article);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
