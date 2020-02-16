using System;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;
using Epok.Persistence.EF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Epok.Composition.Tests
{
    [TestFixture]
    public class InvokerTests
    {
        private SqliteConnection _connection;
        private DomainContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection("datasource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite(_connection)
                .Options;

            Root.InitializeContainer(options);
            _dbContext = Root.Container.GetInstance<DomainContext>();
            _dbContext.Database.EnsureCreated();
        }

        [Test]
        public async Task CommandInvokerShouldResolveCommandHandler()
        {
            var commandInvoker = Root.Container.GetInstance<ICommandInvoker>();
            var command = new RegisterCustomer
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
                InitiatorId = Guid.NewGuid()
            };
            await commandInvoker.Execute(command);
            
            Assert.That(_dbContext.Customers.Find(command.Id), Is.Not.Null);
        }

        [Test]
        [Ignore("TODO")]
        public async Task QueryInvokerShouldResolveQueryHandler()
        {
            throw new NotImplementedException("ToDo");
        }
    }
}
