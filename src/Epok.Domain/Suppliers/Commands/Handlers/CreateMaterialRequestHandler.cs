using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Suppliers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Suppliers.ExceptionCauses;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Creates a new material request with the supplier.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if the supplier does not supplier the
    /// articles requested.
    /// </exception>
    public class CreateMaterialRequestHandler : ICommandHandler<CreateMaterialRequest>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateMaterialRequestHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateMaterialRequest command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.SupplierId);
            var requestedItems = new List<InventoryItem>();
            foreach (var (articleId, amount) in command.Items)
            {
                if (supplier.SuppliableArticles.All(a => a.Id != articleId))
                    throw new DomainException(RequestingUnregisteredArticle(supplier, articleId));
                var article = await _repository.LoadAsync<Article>(articleId);
                requestedItems.Add(new InventoryItem(article, amount));
            }

            var name = $"To {supplier.Name} from {DateTimeOffset.Now}";
            var request = new MaterialRequest(command.Id, name)
            {
                Supplier = supplier,
                Status = MaterialRequestStatus.Submitted,
                ItemsRequested = requestedItems,
                CreatedAt = DateTimeOffset.Now
            };

            supplier.MaterialRequests.Add(request);

            await _repository.AddAsync(request);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<MaterialRequest>(request, Trigger.Added,
                command.InitiatorId));
        }
    }
}
