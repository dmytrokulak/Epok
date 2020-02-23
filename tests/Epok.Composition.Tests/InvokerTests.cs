using System;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Customers.Queries;
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
        public async Task QueryInvokerShouldResolveQueryHandler()
        {
            var customers = Bogus.Customers.Generate(3);
            _dbContext.AddRange(customers);
            _dbContext.SaveChanges();

            var queryInvoker = Root.Container.GetInstance<IQueryInvoker>();

            var query = new CustomersQuery
            {
                FilterIds = customers[0].Id.Collect()
            };
            var response = await queryInvoker.Execute<CustomersQuery, Customer>(query);
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response[0], Is.EqualTo(customers[0]));

            query = new CustomersQuery
            {
                FilterName = customers[0].Name
            };
            response = await queryInvoker.Execute<CustomersQuery, Customer>(query);
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response[0], Is.EqualTo(customers[0]));

            query = new CustomersQuery
            {
                FilterIds = new[] {customers[0].Id, customers[1].Id}
            };
            response = await queryInvoker.Execute<CustomersQuery, Customer>(query);
            Assert.That(response.Count, Is.EqualTo(2));
            Assert.That(response, Is.EquivalentTo(new[] {customers[0], customers[1]}));

            response = await queryInvoker.Execute<CustomersQuery, Customer>(new CustomersQuery());
            Assert.That(response.Count, Is.EqualTo(customers.Count));
            Assert.That(response, Is.EquivalentTo(customers));
        }
    }
}
