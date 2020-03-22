using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Contacts.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Adds new contact to the collection of customer's contacts.
    /// </summary>
    public class ChangeCustomerContactHandler : ICommandHandler<ChangeCustomerContact>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeCustomerContactHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeCustomerContact command)
        {
            var contact = await _repository.LoadAsync<Contact>(command.Id);

            if (contact.FirstName == command.FirstName &&
                contact.LastName == command.LastName &&
                contact.Email == command.Email &&
                contact.PhoneNumber == command.PhoneNumber)
                return;

            //ToDo:4 change name
            contact.FirstName = command.FirstName;
            contact.LastName = command.LastName;
            contact.Email = command.Email;
            contact.PhoneNumber = command.PhoneNumber;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Contact>(contact, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
