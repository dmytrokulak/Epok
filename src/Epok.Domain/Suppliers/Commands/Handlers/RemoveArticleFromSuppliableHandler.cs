using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Removes an article from the collection of suppliable
    /// by the given supplier.
    /// </summary>
    public class RemoveArticleFromSuppliableHandler : ICommandHandler<RemoveArticleFromSuppliable>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public RemoveArticleFromSuppliableHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RemoveArticleFromSuppliable command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.SupplierId);
            var article = await _repository.LoadAsync<Article>(command.ArticleId);

            supplier.SuppliableArticles.Remove(article);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
