using System;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Changes customer's shipping address.
    /// </summary>
    public class ChangeCustomerAddressHandler : ICommandHandler<ChangeCustomerAddress>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeCustomerAddressHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeCustomerAddress command)
        {
            var customer = await _repository.GetAsync<Customer>(command.Id);

            if (customer.ShippingAddress.AddressLine1 == command.AddressLine1 &&
                customer.ShippingAddress.AddressLine2 == command.AddressLine2 &&
                customer.ShippingAddress.City == command.City &&
                customer.ShippingAddress.Province == command.Province &&
                customer.ShippingAddress.Country == command.Country &&
                customer.ShippingAddress.PostalCode == command.PostalCode)
                return;

            customer.ShippingAddress = new Address(Guid.NewGuid(), $"Shipping address for {customer}")
            {
                AddressLine1 = command.AddressLine1,
                AddressLine2 = command.AddressLine2,
                City = command.City,
                Province = command.Province,
                Country = command.Country,
                PostalCode = command.PostalCode,
                CompanyId = customer.Id
            };

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Customer>(customer, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
