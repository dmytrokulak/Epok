using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Allows article to be stored
    /// in shops of the specified category.
    /// </summary>
    public class AllowArticleHandler : ICommandHandler<AllowArticle>
    {
        private readonly IRepository<ShopCategory> _categoryRepo;
        private readonly IArticleRepository _articleRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public AllowArticleHandler(IRepository<ShopCategory> categoryRepo, IArticleRepository articleRepo,
            IEventTransmitter eventTransmitter)
        {
            _categoryRepo = categoryRepo;
            _articleRepo = articleRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AllowArticle command)
        {
            var shopCategory = await _categoryRepo.GetAsync(command.ShopCategoryId);
            var article = await _articleRepo.GetAsync(command.ArticleId);

            shopCategory.Articles.Add(article);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
