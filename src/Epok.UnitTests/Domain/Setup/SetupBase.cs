using Epok.Core.Domain.Entities;
using Epok.Core.Domain.Events;
using Epok.Core.Providers;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Services;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Repositories;
using Epok.Domain.Suppliers.Entities;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Epok.Core.Persistence;
using Epok.Domain.Orders.Entities;

namespace Epok.UnitTests.Domain.Setup
{
    [SetUpFixture]
    public abstract partial class SetupBase
    {
        protected static ITimeProvider TimeProvider = new TimeProvider();

        protected static IEventTransmitter EventTransmitter = A.Fake<IEventTransmitter>();

        protected static IReadOnlyRepository ReadOnlyRepo = A.Fake<IReadOnlyRepository>();
        protected static IRepository<User> UserRepo = A.Fake<IRepository<User>>();
        protected static IRepository<DomainResource> HandlerRepo = A.Fake<IRepository<DomainResource>>();
        protected static IPermissionRepository PermissionRepo = A.Fake<IPermissionRepository>();
        protected static IRepository<Order> OrderRepo = A.Fake<IRepository<Order>>();
        protected static IArticleRepository ArticleRepo = A.Fake<IArticleRepository>();
        protected static IInventoryRepository InventoryRepo = A.Fake<IInventoryRepository>();
        protected static IRepository<Customer> CustomerRepo = A.Fake<IRepository<Customer>>();
        protected static IRepository<Supplier> SupplierRepo = A.Fake<IRepository<Supplier>>();
        protected static IRepository<MaterialRequest> MaterialRequestRepo = A.Fake<IRepository<MaterialRequest>>();
        protected static IRepository<Uom> UomRepo = A.Fake<IRepository<Uom>>();
        protected static IShopRepository ShopRepo = A.Fake<IShopRepository>();
        protected static IRepository<ShopCategory> ShopCategoryRepo = A.Fake<IRepository<ShopCategory>>();
        protected static IRepository<BillOfMaterial> BomRepo = A.Fake<IRepository<BillOfMaterial>>();
        protected static IRepository<SpoilageReport> SpoilageRepo = A.Fake<IRepository<SpoilageReport>>();

        protected static IInventoryService InventoryService = new InventoryService(InventoryRepo, ArticleRepo, TimeProvider);
        protected static IOrderService OrderService = new OrderService(TimeProvider);


        /// <summary>
        /// Runs a single time during a test session.
        /// </summary>
        [OneTimeSetUp]
        protected void SetUpTestEnvironment()
        {
            InitUsers();
            InitInventory();
            InitOrders();
            InitShopCategories();
            InitShops();
            InitCustomers();
            InitSuppliers();

            StubUsersRepositories();
            StubInventoryRepositories();
            StubOrdersRepositories();
            StubShopCategoriesRepositories();
            StubShopsRepositories();
            StubCustomersRepositories();
            StubSuppliersRepositories();
        }

        protected int CallsTo<T>(IRepository<T> repo, string method) where T : IEntity
            => Fake.GetCalls(repo).Count(c => c.Method.Name == method);

        protected List<T> GetRecordedEntities<T>(IRepository<T> repo, string method) where T : IEntity
            => Fake.GetCalls(repo).Where(c => c.Method.Name == method).SelectMany(c => c.Arguments.OfType<T>())
                .ToList();

        protected int RecordedEventCount<T>() where T : IEvent
            => Fake.GetCalls(EventTransmitter)
                .Count(c => c.Arguments.OfType<T>().Any());

        protected List<T> GetRecordedEvents<T>() where T : IEvent
            => Fake.GetCalls(EventTransmitter)
                .SelectMany(c => c.Arguments.OfType<T>())
                .ToList();

        protected List<Guid> GetRecordedIds<T>(IRepository<T> repo, string method) where T : IEntity
            => Fake.GetCalls(repo).Where(c => c.Method.Name == method).SelectMany(c => c.Arguments.OfType<Guid>())
                .ToList();

        protected decimal AmountInStock(Article article)
            => Shops.SelectMany(s => s.Inventory.Where(i => i.Article == article))
                .Sum(i => i.Amount);

        protected decimal AmountInOrders(Article article)
            => Orders.SelectMany(s => s.ItemsOrdered.Where(i => i.Article == article))
                .Sum(i => i.Amount);

        /// <summary>
        /// Runs for after each test.
        /// </summary>
        [TearDown]
        protected void ClearFakesRecordedCalls()
        {
            Fake.ClearRecordedCalls(UserRepo);
            Fake.ClearRecordedCalls(HandlerRepo);
            Fake.ClearRecordedCalls(PermissionRepo);
            Fake.ClearRecordedCalls(OrderRepo);
            Fake.ClearRecordedCalls(InventoryRepo);
            Fake.ClearRecordedCalls(CustomerRepo);
            Fake.ClearRecordedCalls(SupplierRepo);
            Fake.ClearRecordedCalls(MaterialRequestRepo);
            Fake.ClearRecordedCalls(ShopRepo);
            Fake.ClearRecordedCalls(BomRepo);
            Fake.ClearRecordedCalls(EventTransmitter);
        }
    }
}
