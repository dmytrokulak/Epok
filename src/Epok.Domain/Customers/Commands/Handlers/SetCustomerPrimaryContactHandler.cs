using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Customers.Entities;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Sets the primary contact of the customer.
    /// </summary>
    public class SetCustomerPrimaryContactHandler : ICommandHandler<SetCustomerPrimaryContact>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public SetCustomerPrimaryContactHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(SetCustomerPrimaryContact command)
        {
            var customer = await _repository.GetAsync<Customer>(command.CustomerId);
            if (customer.PrimaryContact.Id == command.NewPrimaryContactId)
                return;

            customer.PrimaryContact.Primary = false;
            customer.Contacts.Single(c => c.Id == command.NewPrimaryContactId).Primary = true;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Customer>(customer, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
