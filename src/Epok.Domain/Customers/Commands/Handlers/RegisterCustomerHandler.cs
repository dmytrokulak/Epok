using System;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Registers new customer with the system. 
    /// Creates a dedicate postal address and a primary contact.
    /// Transmits a corresponding event.
    /// </summary>
    public class RegisterCustomerHandler : ICommandHandler<RegisterCustomer>
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterCustomerHandler(IRepository<Customer> customerRepo,
            IEventTransmitter eventTransmitter)
        {
            _customerRepo = customerRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterCustomer command)
        {
            var contact = new Contact(Guid.NewGuid(), $"{command.Name} Primary contact")
            {
                FirstName = command.PrimaryContactFirstName,
                LastName = command.PrimaryContactLastName,
                PhoneNumber = command.PrimaryContactPhone,
                Email = command.PrimaryContactEmail,
                Primary = true
            };

            var address = new Address(command.Id, $"{command.Name} shipping address.")
            {
                AddressLine1 = command.ShippingAddressLine1,
                AddressLine2 = command.ShippingAddressLine2,
                City = command.ShippingAddressCity,
                Province = command.ShippingAddressProvince,
                Country = command.ShippingAddressCountry,
                PostalCode = command.ShippingAddressPostalCode
            };

            var customer = new Customer(command.Id, command.Name)
            {
                CustomerType = command.Type,
                Contacts = contact.Collect().ToHashSet(),
                ShippingAddress = address
            };

            await _customerRepo.AddAsync(customer);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Customer>(customer, Trigger.Added,
                command.InitiatorId));
        }
    }
}
