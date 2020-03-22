using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
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
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public AddBillOfMaterialHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(AddBillOfMaterial command)
        {
            var article = await _repository.GetAsync<Article>(command.ArticleId);

            var input = (await _repository.LoadSomeAsync<Article>(command.Input.Select(i => i.articleId)))
                .Select(a => new InventoryItem(a, command.Input.Single(i => i.articleId == a.Id).amount)).ToHashSet();

            var bom = article.BillsOfMaterial.FirstOrDefault(b => b.Input.SetEquals(input));
            if (bom != null)
                throw new DomainException(IdenticalBomExists(bom));

            var newBom = new BillOfMaterial(command.Id, command.Name)
            {
                Article = article,
                Input = input,
                Output = command.Output,
                Primary = false
            };

            await _repository.AddAsync(newBom);
            article.BillsOfMaterial.Add(newBom);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(newBom, Trigger.Added,
                command.InitiatorId));
        }
    }
}
