using System;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Changes Supplier's shipping address.
    /// </summary>
    public class ChangeSupplierAddressHandler : ICommandHandler<ChangeSupplierAddress>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeSupplierAddressHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeSupplierAddress command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.Id);

            if (supplier.ShippingAddress.AddressLine1 == command.AddressLine1 &&
                supplier.ShippingAddress.AddressLine2 == command.AddressLine2 &&
                supplier.ShippingAddress.City == command.City &&
                supplier.ShippingAddress.Province == command.Province &&
                supplier.ShippingAddress.Country == command.Country &&
                supplier.ShippingAddress.PostalCode == command.PostalCode)
                return;

            supplier.ShippingAddress = new Address(Guid.NewGuid(), $"Shipping address for {supplier}")
            {
                AddressLine1 = command.AddressLine1,
                AddressLine2 = command.AddressLine2,
                City = command.City,
                Province = command.Province,
                Country = command.Country,
                PostalCode = command.PostalCode,
                CompanyId = supplier.Id
            };

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
