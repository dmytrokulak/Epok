using Epok.Core.Domain.Entities;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Core.Providers;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Services;
using Epok.Domain.Shops.Repositories;
using Epok.Domain.Users.Repositories;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Tests.Setup
{
    [SetUpFixture]
    public abstract partial class SetupBase
    {
        protected static ITimeProvider TimeProvider = new TimeProvider();

        protected static IEventTransmitter EventTransmitter = A.Fake<IEventTransmitter>();

        protected static IEntityRepository EntityRepository = A.Fake<IEntityRepository>();
        protected static IPermissionRepository PermissionRepo = A.Fake<IPermissionRepository>();
        protected static IArticleRepository ArticleRepo = A.Fake<IArticleRepository>();
        protected static IInventoryRepository InventoryRepo = A.Fake<IInventoryRepository>();
        protected static IShopRepository ShopRepo = A.Fake<IShopRepository>();

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

        protected int CallsTo(IRepository repo, string method)
            => Fake.GetCalls(repo).Count(c => c.Method.Name == method);

        protected List<T> GetRecordedEntities<T>(IRepository repo, string method) where T : IEntity
            => Fake.GetCalls(repo).Where(c => c.Method.Name == method).SelectMany(c => c.Arguments.OfType<T>())
                .ToList();

        protected int RecordedEventCount<T>() where T : IEvent
            => Fake.GetCalls(EventTransmitter)
                .Count(c => c.Arguments.OfType<T>().Any());

        protected List<T> GetRecordedEvents<T>() where T : IEvent
            => Fake.GetCalls(EventTransmitter)
                .SelectMany(c => c.Arguments.OfType<T>())
                .ToList();

        protected List<Guid> GetRecordedIds(IRepository repo, string method)
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
            Fake.ClearRecordedCalls(EntityRepository);
            Fake.ClearRecordedCalls(PermissionRepo);
            Fake.ClearRecordedCalls(InventoryRepo);
            Fake.ClearRecordedCalls(ShopRepo);
            Fake.ClearRecordedCalls(EventTransmitter);
        }
    }
}
