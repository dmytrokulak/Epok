using Epok.Core.Persistence;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;
using Epok.Persistence.EF.Repositories;
using Logging.Memory;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace Epok.Persistence.EF.Tests
{
    [TestFixture]
    public class EntityRepositoryTests
    {
        private DomainContext _dbContext;
        private SqliteConnection _connection;
        private IList<Guid> _customerIds;
        private IEntityIdentifiersKeeper _idsKeeper;

        private static readonly ILoggerFactory LogFactory
            = LoggerFactory.Create(config =>
                config.AddFilter(DbLoggerCategory.Database.Command.Name,
                    LogLevel.Information)).AddMemory();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new SqliteConnection("datasource=:memory:");
            _connection.Open();

            InitDbContext();
            _dbContext.Database.EnsureCreated();

            var seedData = Bogus.Customers.Generate(10);
            _dbContext.Customers.AddRange(seedData);
            _dbContext.SaveChanges();
            _dbContext.Dispose();
            CleanMemoryLogger();

            _customerIds = seedData.Select(c => c.Id).ToList();
        }

        [SetUp]
        public void SetUp()
        {
            InitDbContext();
            _idsKeeper = new EntityIdentifiersKeeper();
        }

        private void InitDbContext()
        {
            var options = new DbContextOptionsBuilder<DomainContext>()
                .UseSqlite(_connection)
                .UseLoggerFactory(LogFactory)
                .EnableSensitiveDataLogging()
                .Options;

            _dbContext = new DomainContext(options);
        }

        private readonly Regex _selectCustomersJoinAddresses =
            new Regex(@"SELECT(.|\n){1,}Customers(.|\n){1,}JOIN(.|\n){1,}Addresses", RegexOptions.IgnoreCase);

        private readonly Regex _selectCustomersProper =
            new Regex(@"SELECT(.|\n){1,}Customers(?!(.|\n){1,}JOIN)", RegexOptions.IgnoreCase);

        private readonly Regex _selectCustomers =
            new Regex(@"SELECT(.|\n){1,}Customers", RegexOptions.IgnoreCase);

        private readonly Regex _selectAddresses =
            new Regex(@"SELECT(.|\n){1,}Addresses", RegexOptions.IgnoreCase);

        [Test]
        public async Task LoadShouldNotQueryForTheEntityLoadedIntoMemory()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);

            var customer = await repo.LoadAsync<Customer>(_customerIds[0]);
            Assert.That(customer, Is.Not.Null);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            customer = await repo.LoadAsync<Customer>(_customerIds[0]);
            Assert.That(customer, Is.Not.Null);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));
        }

        [Test]
        public async Task LoadSomeShouldNotQueryForEntitiesLoadedIntoMemory()
        {
            var ids = _customerIds.Take(3).ToList();
            var repo = new EntityRepository(_dbContext, _idsKeeper);

            var customers = await repo.LoadSomeAsync<Customer>(ids);
            Assert.That(customers, Is.Not.Empty);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            customers = await repo.LoadSomeAsync<Customer>(ids);
            Assert.That(customers, Is.Not.Empty);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));
        }

        [Test]
        public async Task LoadAllShouldNotQueryForEntitiesLoadedIntoMemory()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);

            var customers = await repo.LoadAllAsync<Customer>();
            Assert.That(customers, Is.Not.Empty);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            customers = await repo.LoadAllAsync<Customer>();
            Assert.That(customers, Is.Not.Empty);
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));
        }

        [Test]
        public async Task GetShouldUseEagerLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customer = await repo.GetAsync<Customer>(_customerIds[0]);

            //assert that EF makes a single "customer select" call to db
            //with this call incorporating sub queries to populate properties
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersJoinAddresses.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address = customer.ShippingAddress;
            Assert.That(address, Is.Not.Null);

            //assert that EF does not make extra calls to db
            //and the only "address select" is the one joined on "customer select"
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(1));
            Assert.That(addressSelect[0], Is.EqualTo(customerSelect[0]));
        }

        [Test]
        public async Task GetSomeShouldUseEagerLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.GetSomeAsync<Customer>(_customerIds.Take(5));

            Assert.That(customers.Count, Is.EqualTo(5));

            //a bogus action on each loaded item
            foreach (var item in customers)
                item.CustomerType.ToString();

            //assert that EF makes a single "customer select" call to db
            //with this call incorporating sub queries to populate properties
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersJoinAddresses.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address = customers[0].ShippingAddress;
            Assert.That(address, Is.Not.Null);

            //assert that EF does not make extra calls to db
            //and the only "address select" is the one joined on "customer select"
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(1));
            Assert.That(addressSelect[0], Is.EqualTo(customerSelect[0]));
        }

        [Test]
        public async Task GetAllShouldUseEagerLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.GetAllAsync<Customer>();

            Assert.That(customers.Count, Is.EqualTo(_customerIds.Count));

            //a bogus action on each loaded item
            foreach (var item in customers)
                item.CustomerType.ToString();

            //assert that EF makes a single "customer select" call to db
            //with this call incorporating sub queries to populate properties
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersJoinAddresses.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address = customers[0].ShippingAddress;
            Assert.That(address, Is.Not.Null);

            //assert that EF does not make extra calls to db
            //and the only "address select" is the one joined on "customer select"
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(1));
            Assert.That(addressSelect[0], Is.EqualTo(customerSelect[0]));
        }

        [Test]
        public async Task LoadShouldUseLazyLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customer = await repo.LoadAsync<Customer>(_customerIds[0]);

            //assert select without joins
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersProper.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address = customer.ShippingAddress;
            Assert.That(address, Is.Not.Null);

            //assert that EF does makes a call to db
            //to select and address at first usage of the property
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(1));
            Assert.That(addressSelect[0], Is.Not.EqualTo(customerSelect[0]));
        }

        [Test]
        public async Task LoadSomeShouldUseLazyLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.LoadSomeAsync<Customer>(_customerIds.Take(5));

            Assert.That(customers.Count, Is.EqualTo(5));

            //a bogus action on each loaded item
            foreach (var item in customers)
                item.CustomerType.ToString();

            //assert select many without joins
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersProper.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address0 = customers[0].ShippingAddress;
            Assert.That(address0, Is.Not.Null);

            var address1 = customers[1].ShippingAddress;
            Assert.That(address1, Is.Not.Null);

            //assert that EF does not make extra calls to db
            //and the only "address select" is the one joined on "customer select"
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(2));
            //ToDo: trim time
            Assert.That(addressSelect[0], Is.Not.EqualTo(customerSelect[0]));
            Assert.That(addressSelect[0], Is.Not.EqualTo(addressSelect[1]));
        }

        [Test]
        public async Task LoadAllShouldUseLazyLoading()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.LoadAllAsync<Customer>();

            Assert.That(customers.Count, Is.EqualTo(_customerIds.Count));

            //a bogus action on each loaded item
            foreach (var item in customers)
                item.CustomerType.ToString();

            //assert select many without joins
            var customerSelect = MemoryLogger.LogList.Where(l => _selectCustomersProper.IsMatch(l)).ToList();
            Assert.That(customerSelect.Count, Is.EqualTo(1));
            Assert.That(MemoryLogger.LogList.Count(l => _selectCustomers.IsMatch(l)), Is.EqualTo(1));

            var address0 = customers[0].ShippingAddress;
            Assert.That(address0, Is.Not.Null);

            var address1 = customers[1].ShippingAddress;
            Assert.That(address1, Is.Not.Null);

            //assert that EF does not make extra calls to db
            //and the only "address select" is the one joined on "customer select"
            var addressSelect = MemoryLogger.LogList.Where(l => _selectAddresses.IsMatch(l)).ToList();
            Assert.That(addressSelect.Count, Is.EqualTo(2));
            //ToDo: trim time
            Assert.That(addressSelect[0], Is.Not.EqualTo(customerSelect[0]));
            Assert.That(addressSelect[0], Is.Not.EqualTo(addressSelect[1]));
        }


        [Test]
        public async Task AddShouldInsertAndDeleteRelatedEntities()
        {
            var work = new UnitOfWorkFactory<UnitOfWork>(_dbContext, _idsKeeper);
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customer = Bogus.Customers.Generate(1).Single();

            using (work.Transact())
                await repo.AddAsync(customer);
            var address = await repo.GetAsync<Address>(customer.ShippingAddress.Id);
            Assert.That(address, Is.Not.Null);
            var contact = await repo.GetAsync<Contact>(customer.PrimaryContact.Id);
            Assert.That(contact, Is.Not.Null);

            using (work.Transact())
                await repo.RemoveAsync(customer);
            var customerInDb = await repo.GetAsync<Customer>(customer.Id);
            Assert.That(customerInDb, Is.Null);
            address = await repo.GetAsync<Address>(customer.ShippingAddress.Id);
            Assert.That(address, Is.Null);
            contact = await repo.GetAsync<Contact>(customer.PrimaryContact.Id);
            Assert.That(contact, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            CleanMemoryLogger();
            _dbContext.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => _connection.Close();


        /// <summary>
        /// MemoryLogger does not provide for log clean-up.
        /// This workaround is to compensate for the missing feature.
        /// Only for testing purposes.
        /// </summary>
        private static void CleanMemoryLogger()
        {
            var field = typeof(MemoryLogger).GetField("logsDictionary", BindingFlags.Static | BindingFlags.NonPublic);
            var dict = (IDictionary)field?.GetValue(field);
            if (dict == null) return;
            foreach (var value in dict.Values)
            {
                var childField =
                    value.GetType().GetField("logList", BindingFlags.Instance | BindingFlags.NonPublic);
                var list = (IList)childField?.GetValue(value);
                list?.Clear();
            }
        }
    }
}