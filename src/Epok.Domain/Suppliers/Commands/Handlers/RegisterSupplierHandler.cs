using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Suppliers.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Registers a supplier in the system.
    /// </summary>
    public class RegisterSupplierHandler : ICommandHandler<RegisterSupplier>
    {
        private readonly IRepository<Supplier> _supplierRepo;
        private readonly IArticleRepository _articleRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterSupplierHandler(IRepository<Supplier> supplierRepo, IArticleRepository articleRepo,
            IEventTransmitter eventTransmitter)
        {
            _supplierRepo = supplierRepo;
            _articleRepo = articleRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterSupplier command)
        {
            var contact = new Contact
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

            var articles = await _articleRepo.GetSomeAsync(command.SuppliableArticleIds);

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
