using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Suppliers.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Registers a supplier in the system.
    /// </summary>
    public class RegisterSupplierHandler : ICommandHandler<RegisterSupplier>
    {
        private readonly IRepository<Supplier> _supplierRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterSupplierHandler(IRepository<Supplier> supplierRepo, IInventoryRepository inventoryRepo,
            IEventTransmitter eventTransmitter)
        {
            _supplierRepo = supplierRepo;
            _inventoryRepo = inventoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterSupplier command)
        {
            var contact = new Contact(
                new PersonName(command.PrimaryContactFirstName, command.PrimaryContactLastName),
                command.PrimaryContactPhone, command.PrimaryContactEmail, true);

            var address = new Address(command.Id, $"{command.Name} shipping address.")
            {
                AddressLine1 = command.ShippingAddressLine1,
                AddressLine2 = command.ShippingAddressLine2,
                City = command.ShippingAddressCity,
                Province = command.ShippingAddressProvince,
                Country = command.ShippingAddressCountry,
                PostalCode = command.ShippingAddressPostalCode
            };

            var articles = await _inventoryRepo.GetSomeAsync(command.SuppliableArticleIds);

            var supplier = new Supplier(command.Id, command.Name)
            {
                Contacts = contact.Collect().ToHashSet(),
                ShippingAddress = address,
                SuppliableArticles = articles.ToHashSet()
            };

            await _supplierRepo.AddAsync(supplier);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Added,
                command.InitiatorId));
        }
    }
}
