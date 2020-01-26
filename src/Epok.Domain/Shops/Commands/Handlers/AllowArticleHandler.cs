using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
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
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public AllowArticleHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AllowArticle command)
        {
            var shopCategory = await _repository.GetAsync<ShopCategory>(command.ShopCategoryId);
            var article = await _repository.GetAsync<Article>(command.ArticleId);

            shopCategory.Articles.Add(article);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
