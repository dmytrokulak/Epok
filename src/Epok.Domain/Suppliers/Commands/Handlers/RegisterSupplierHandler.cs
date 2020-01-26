using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Suppliers.Entities;
using System;
using System.Threading.Tasks;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Registers a supplier in the system.
    /// </summary>
    public class RegisterSupplierHandler : ICommandHandler<RegisterSupplier>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterSupplierHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterSupplier command)
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

            var articles = await _repository.GetSomeAsync<Article>(command.SuppliableArticleIds);

            var supplier = new Supplier(command.Id, command.Name)
            {
                Contacts = contact.Collect().ToHashSet(),
                ShippingAddress = address,
                SuppliableArticles = articles.ToHashSet()
            };

            await _repository.AddAsync(supplier);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Added,
                command.InitiatorId));
        }
    }
}
