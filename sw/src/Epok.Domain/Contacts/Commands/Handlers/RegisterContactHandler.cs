using System;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Contacts.Commands.Handlers
{
    /// <summary>
    /// Adds new contact to the collection of customer's/supplier's contacts.
    /// </summary>
    public class RegisterContactHandler : ICommandHandler<RegisterContact>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterContactHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterContact command)
        {
            var customer = await _repository.LoadAsync<Customer>(command.CompanyId);
            var supplier = await _repository.LoadAsync<Supplier>(command.CompanyId);
            var companyName = customer?.Name ?? supplier.Name;

            var contact = new Contact(command.Id, $"{command.FirstName} {command.LastName} of {companyName}.")
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.PhoneNumber,
                Email = command.Email,
                Primary = false,
                CompanyId = command.CompanyId
            };

            await _repository.AddAsync(contact);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Contact>(contact, Trigger.Added,
                command.InitiatorId));
        }
    }
}
