using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Suppliers.Entities;
using static Epok.Domain.Suppliers.ExceptionCauses;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Adds an article to the collection of suppliable
    /// by the given supplier.
    /// </summary>
    public class AddArticleToSuppliableHandler : ICommandHandler<AddArticleToSuppliable>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public AddArticleToSuppliableHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AddArticleToSuppliable command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.SupplierId);
            var article = await _repository.LoadAsync<Article>(command.ArticleId);

            if (article.ArticleType >= ArticleType.Product)
                throw new DomainException(SupplyingFinishedProduct(article, supplier));

            supplier.SuppliableArticles.Add(article);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
