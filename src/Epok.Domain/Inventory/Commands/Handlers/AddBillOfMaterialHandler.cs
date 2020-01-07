using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Creates a new bill of material for the article.
    /// </summary>
    /// <exception cref="DomainException">
    /// Throws if a bill of material with the same input
    /// exists for the article.
    /// </exception>
    public class AddBillOfMaterialHandler : ICommandHandler<AddBillOfMaterial>
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IRepository<BillOfMaterial> _bomRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public AddBillOfMaterialHandler(IInventoryRepository inventoryRepo,
            IRepository<BillOfMaterial> bomRepo, IEventTransmitter eventTransmitter)
        {
            _inventoryRepo = inventoryRepo;
            _bomRepo = bomRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AddBillOfMaterial command)
        {
            var article = await _inventoryRepo.GetAsync(command.ArticleId);

            var input = new HashSet<InventoryItem>();
            foreach (var (articleId, amount) in command.Input)
                input.Add(new InventoryItem(await _inventoryRepo.LoadAsync(articleId), amount));
           
            var bom = article.BillsOfMaterial
                                          .FirstOrDefault(b => b.Input.SetEquals(input));
            if (bom != null)
                throw new DomainException(IdenticalBomExists(bom));
          
            var newBom = new BillOfMaterial(command.Id, command.Name)
            {
                Article = article,
                Input = input,
                Output = command.Output,
                Primary = false
            };

            await _bomRepo.AddAsync(newBom);
            article.BillsOfMaterial.Add(newBom);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(newBom, Trigger.Added,
                command.InitiatorId));
        }
    }
}
