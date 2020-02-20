using Epok.Core.Persistence;
using Epok.Domain.Customers.Entities;
using Epok.Persistence.EF.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Tests
{
    [TestFixture]
    public class EntityIdentifiersKeeperTests
    {
        private DomainContext _dbContext;
        private SqliteConnection _connection;
        private IEntityIdentifiersKeeper _idsKeeper;

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
                .Options;

            _dbContext = new DomainContext(options);
        }


        [Test]
        public async Task ShouldUpdateKeeperAtGetAll()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.GetAllAsync<Customer>();
            Assert.That(customers, Is.Not.Empty);
            Assert.That(_idsKeeper.Get<Customer>(), Is.EquivalentTo(customers.Select(c => c.Id)));
        }

        [Test]
        public async Task ShouldUpdateKeeperAtLoadAll()
        {
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.LoadAllAsync<Customer>();
            Assert.That(customers, Is.Not.Empty);
            Assert.That(_idsKeeper.Get<Customer>(), Is.EquivalentTo(customers.Select(c => c.Id)));
        }

        [Test]
        public async Task ShouldRemoveIdFromKeeperAtSaveChanges()
        {
            var work = new UnitOfWorkFactory<UnitOfWork>(_dbContext, _idsKeeper);
            var repo = new EntityRepository(_dbContext, _idsKeeper);
            var customers = await repo.GetAllAsync<Customer>();

            var customer = customers[0];
            using (work.Transact())
                await repo.RemoveAsync(customer);

            Assert.That(_idsKeeper.Get<Customer>().Contains(customer.Id), Is.False);
        }

        [Test]
        public async Task ShouldAddIdToKeeperAtSaveChanges()
        {
            var work = new UnitOfWorkFactory<UnitOfWork>(_dbContext, _idsKeeper);
            var repo = new EntityRepository(_dbContext, _idsKeeper);

            var newCustomer = Bogus.Customers.Generate(1).Single();
            using (work.Transact())
                await repo.AddAsync(newCustomer);

            Assert.That(_idsKeeper.Get<Customer>().Contains(newCustomer.Id), Is.True);
        }

        [TearDown]
        public void TearDown()
            => _dbContext.Dispose();

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => _connection.Close();
    }
}
