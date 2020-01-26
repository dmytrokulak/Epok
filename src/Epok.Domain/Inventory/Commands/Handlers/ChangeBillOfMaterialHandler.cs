using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Modifies input and\or output of the bill of material.
    /// </summary>
    public class ChangeBillOfMaterialHandler : ICommandHandler<ChangeBillOfMaterial>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeBillOfMaterialHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeBillOfMaterial command)
        {
            var bomLoaded = await _repository.GetAsync<BillOfMaterial>(command.Id);
            Guard.Against.Null(bomLoaded, nameof(bomLoaded));

            var input = new HashSet<InventoryItem>();
            foreach (var (articleId, amount) in command.Input)
            {
                var article = await _repository.LoadAsync<Article>(articleId);
                input.Add(new InventoryItem(article, amount));
            }

            bomLoaded.Input = input;
            bomLoaded.Output = command.Output;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(bomLoaded, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
