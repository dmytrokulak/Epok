using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Sets a primary (default) bill of material for the article.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if the bill of material is already being set as primary.
    /// </exception>
    public class SetPrimaryBillOfMaterialHandler : ICommandHandler<SetPrimaryBillOfMaterial>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public SetPrimaryBillOfMaterialHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(SetPrimaryBillOfMaterial command)
        {
            var article = await _repository.GetAsync<Article>(command.ArticleId);
            var current = article.PrimaryBillOfMaterial;
            if (current.Id == command.BomId)
                throw new DomainException(BomIsAlreadyPrimary(current));

            current.Primary = false;
            var incoming = article.BillsOfMaterial.Single(b => b.Id == command.BomId);
            incoming.Primary = true;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(current, Trigger.Changed,
                command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(incoming, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
