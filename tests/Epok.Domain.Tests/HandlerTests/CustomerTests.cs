using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;
using Epok.Domain.Customers.Commands.Handlers;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Tests.Setup;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Customers.ExceptionCauses;

namespace Epok.Domain.Tests.HandlerTests
{
    [TestFixture]
    public class CustomerTests : SetupBase
    {
        [Test]
        public async Task RegisterCustomer_Success()
        {
            var command = new RegisterCustomer()
            {
                Name = "Customer Buyer",
                Type = CustomerType.RetailOutlet,
                PrimaryContactFirstName = "Sam",
                PrimaryContactLastName = "Gold",
                PrimaryContactPhone = "380000000000",
                PrimaryContactEmail = "sam.gold@mail.me",
                ShippingAddressLine1 = "1 Paper st.",
                ShippingAddressLine2 = "1 Apt.",
                ShippingAddressCity = "Kyiv",
                ShippingAddressProvince = "Kyiv",
                ShippingAddressCountry = "Ukraine",
                ShippingAddressPostalCode = "37007",
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new RegisterCustomerHandler(EntityRepository, EventTransmitter);
            await handler.HandleAsync(command);

            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.AddAsync)), Is.EqualTo(1));
            var entity = GetRecordedEntities<Customer>(EntityRepository, nameof(EntityRepository.AddAsync)).Single();
            Assert.That(entity.Name, Is.EqualTo(command.Name));
            Assert.That(entity.CustomerType, Is.EqualTo(command.Type));
            Assert.That(entity.PrimaryContact.FirstName, Is.EqualTo(command.PrimaryContactFirstName));
            Assert.That(entity.PrimaryContact.LastName, Is.EqualTo(command.PrimaryContactLastName));
            Assert.That(entity.PrimaryContact.PhoneNumber, Is.EqualTo(command.PrimaryContactPhone));
            Assert.That(entity.PrimaryContact.Email, Is.EqualTo(command.PrimaryContactEmail));
            Assert.That(entity.ShippingAddress.AddressLine1, Is.EqualTo(command.ShippingAddressLine1));
            Assert.That(entity.ShippingAddress.AddressLine2, Is.EqualTo(command.ShippingAddressLine2));
            Assert.That(entity.ShippingAddress.City, Is.EqualTo(command.ShippingAddressCity));
            Assert.That(entity.ShippingAddress.Province, Is.EqualTo(command.ShippingAddressProvince));
            Assert.That(entity.ShippingAddress.Country, Is.EqualTo(command.ShippingAddressCountry));
            Assert.That(entity.ShippingAddress.PostalCode, Is.EqualTo(command.ShippingAddressPostalCode));

            var events = GetRecordedEvents<DomainEvent<Customer>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(entity));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public async Task ArchiveCustomer_Success()
        {
            var command = new ArchiveCustomer
            {
                Id = CustomerToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveCustomerHandler(EntityRepository, EventTransmitter);

            await handler.HandleAsync(command);

            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.RemoveAsync)), Is.EqualTo(1));

            var events = GetRecordedEvents<DomainEvent<Customer>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(CustomerToArchive));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public void ArchiveCustomer_FailFor_CustomerHasActiveOrders()
        {
            var command = new ArchiveCustomer
            {
                Id = CustomerDoorsBuyer.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveCustomerHandler(EntityRepository, EventTransmitter);

            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));

            Assert.That(ex.Message, Is.EqualTo(ArchivingCustomerWithActiveOrders(CustomerDoorsBuyer)));
            Assert.That(GetRecordedEvents<DomainEvent<Customer>>(), Is.Empty);
        }
    }
}
