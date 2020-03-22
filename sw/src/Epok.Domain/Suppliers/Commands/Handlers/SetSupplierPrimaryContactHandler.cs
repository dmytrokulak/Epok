using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Sets the primary contact of the Supplier.
    /// </summary>
    public class SetSupplierPrimaryContactHandler : ICommandHandler<SetSupplierPrimaryContact>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public SetSupplierPrimaryContactHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(SetSupplierPrimaryContact command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.SupplierId);
            if (supplier.PrimaryContact.Id == command.NewPrimaryContactId)
                return;

            supplier.PrimaryContact.Primary = false;
            supplier.Contacts.Single(c => c.Id == command.NewPrimaryContactId).Primary = true;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
