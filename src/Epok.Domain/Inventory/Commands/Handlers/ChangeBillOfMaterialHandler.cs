using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Utilities;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Modifies input and\or output of the bill of material.
    /// </summary>
    public class ChangeBillOfMaterialHandler : ICommandHandler<ChangeBillOfMaterial>
    {
        private readonly IRepository<BillOfMaterial> _bomRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeBillOfMaterialHandler(IRepository<BillOfMaterial> bomRepo,
            IInventoryRepository inventoryRepo,
            IEventTransmitter eventTransmitter)
        {
            _bomRepo = bomRepo;
            _inventoryRepo = inventoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeBillOfMaterial command)
        {
            var bomLoaded = await _bomRepo.GetAsync(command.Id);
            Guard.Against.Null(bomLoaded, nameof(bomLoaded));

            var input = new HashSet<InventoryItem>();
            foreach (var (articleId, amount) in command.Input)
            {
                var article = await _inventoryRepo.LoadAsync(articleId);
                input.Add(new InventoryItem(article, amount));
            }

            bomLoaded.Input = input;
            bomLoaded.Output = command.Output;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(bomLoaded, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
