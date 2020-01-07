using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders;
using Epok.Domain.Orders.Commands;
using Epok.Domain.Orders.Commands.Handlers;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class OrderTests : SetupBase
    {
        [Test]
        public async Task CreateOrder_Success()
        {
            //arrange
            const decimal amount = 5;
            var command = new CreateOrder()
            {
                Name = "New order for product 1",
                CustomerId = CustomerDoorsBuyer.Id,
                Items = new List<(Guid, decimal)> {(Product1InteriorDoor.Id, amount)},
                ShipmentDeadline = DateTimeOffset.Parse("2022-01-01"),
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateOrderHandler(OrderRepo, ReadOnlyRepo, InventoryService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(OrderRepo, nameof(OrderRepo.AddAsync)), Is.EqualTo(1));
            var entity = GetRecordedEntities(OrderRepo, nameof(OrderRepo.AddAsync)).Single();
            Assert.That(entity.Name, Is.EqualTo(command.Name));
            Assert.That(entity.ItemsOrdered.Count, Is.EqualTo(1));
            Assert.That(entity.ItemsProduced.Count, Is.EqualTo(1));
            Assert.That(entity.ItemsOrdered.Of(Product1InteriorDoor).Amount, Is.EqualTo(amount));
            Assert.That(entity.ItemsProduced.Of(Product1InteriorDoor).Amount, Is.Zero);
            Assert.That(entity.Customer, Is.EqualTo(CustomerDoorsBuyer));
            Assert.That(entity.ParentOrder, Is.Null);
            Assert.That(entity.ShipmentDeadline, Is.EqualTo(command.ShipmentDeadline));
            Assert.That(entity.ShippedAt, Is.Null);
            Assert.That(entity.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(entity.Type, Is.EqualTo(command.OrderType));
            Assert.That(entity.Shop, Is.Null);
            Assert.That(entity.WorkStartedAt, Is.Null);
            Assert.That(entity.SubOrders, Is.Empty);
            Assert.That(CustomerDoorsBuyer.Orders.Contains(entity), Is.True);
            Assert.That(GetRecordedEvents<DomainEvent<Order>>().Count, Is.EqualTo(1));

            //annul
            CustomerDoorsBuyer.Orders.Remove(entity);
        }

        [Test]
        public void CreateOrder_FailFor_LackOfMaterials()
        {
            //arrange
            var command = new CreateOrder()
            {
                CustomerId = CustomerDoorsBuyer.Id,
                Items = new List<(Guid, decimal)> {(Product1InteriorDoor.Id, 100)},
                ShipmentDeadline = DateTimeOffset.Parse("2022-01-01"),
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateOrderHandler(OrderRepo, ReadOnlyRepo, InventoryService, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message.StartsWith($"Required {100} {Product1InteriorDoor.UoM} of {Product1InteriorDoor}"),
                Is.True);
            Assert.That(GetRecordedEvents<DomainEvent<Order>>(), Is.Empty);
        }

        [Test]
        public void CreateOrder_FailFor_LackOfTime()
        {
            //arrange
            var now = DateTimeOffset.Now; //ToDo:4 get rid  of real time dependency
            var command = new CreateOrder()
            {
                Name = "New order",
                CustomerId = CustomerDoorsBuyer.Id,
                Items = new List<(Guid, decimal)> {(Product1InteriorDoor.Id, 5)},
                ShipmentDeadline = now,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateOrderHandler(OrderRepo, ReadOnlyRepo, InventoryService, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message.StartsWith("Not enough time to fulfill the order"), Is.True);
            Assert.That(GetRecordedEvents<DomainEvent<Order>>(), Is.Empty);
        }


        [Test]
        public async Task CreateSubOrders_Success()
        {
            //adhere
            var subOrdersInitial = Product1Order.SubOrders.DeepCopy();
            var assemblyShopOrders = ProductAssemblyShop.Orders.DeepCopy();
            var timberShopOrders = TimberComponentShop.Orders.DeepCopy();
            var mdfShopOrders = MdfComponentShop.Orders.DeepCopy();

            //arrange
            var amount = Product1Order.ItemsOrdered.Of(Product1InteriorDoor).Amount;
            var command = new CreateSubOrders
            {
                OrderId = Product1Order.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateSubOrdersHandler(OrderRepo, InventoryRepo, OrderService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities(OrderRepo, nameof(OrderRepo.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(4));

            var doorsOrder = entities.Single(o => o.ItemsOrdered.Any(i => i.Article == Product1InteriorDoor));
            Assert.That(doorsOrder.ItemsOrdered.Count, Is.EqualTo(1));
            Assert.That(doorsOrder.ItemsProduced.Count, Is.EqualTo(1));
            Assert.That(doorsOrder.ItemsOrdered.Of(Product1InteriorDoor).Amount, Is.EqualTo(amount));
            Assert.That(doorsOrder.ItemsProduced.Of(Product1InteriorDoor).Amount, Is.Zero);
            Assert.That(doorsOrder.Customer, Is.EqualTo(Product1Order.Customer));
            Assert.That(doorsOrder.ParentOrder, Is.EqualTo(Product1Order));
            Assert.That(doorsOrder.ReferenceOrder, Is.EqualTo(Product1Order));
            var deadline = TimeProvider.Subtract(doorsOrder.ParentOrder.ShipmentDeadline,
                doorsOrder.ItemsOrdered.Single().TimeToProduce);
            Assert.That(doorsOrder.ShipmentDeadline, Is.EqualTo(deadline));
            Assert.That(doorsOrder.ShippedAt, Is.Null);
            Assert.That(doorsOrder.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(doorsOrder.Type, Is.EqualTo(OrderType.Internal));
            Assert.That(doorsOrder.Shop, Is.EqualTo(ProductAssemblyShop));
            Assert.That(doorsOrder.WorkStartedAt, Is.Null);
            Assert.That(doorsOrder.SubOrders, Is.Not.Empty);

            var verticalOrder = entities.Single(o => o.ItemsOrdered.Any(i => i.Article == Component1Vertical));
            Assert.That(verticalOrder.ItemsOrdered.Count, Is.EqualTo(1));
            Assert.That(verticalOrder.ItemsProduced.Count, Is.EqualTo(1));
            var verticalAmount =
                Product1InteriorDoor.PrimaryBillOfMaterial.Input.Of(Component1Vertical).Amount * amount;
            Assert.That(verticalOrder.ItemsOrdered.Of(Component1Vertical).Amount, Is.EqualTo(verticalAmount));
            Assert.That(verticalOrder.ItemsProduced.Of(Component1Vertical).Amount, Is.Zero);
            Assert.That(verticalOrder.Customer, Is.EqualTo(Product1Order.Customer));
            Assert.That(verticalOrder.ParentOrder, Is.EqualTo(doorsOrder));
            Assert.That(verticalOrder.ReferenceOrder, Is.EqualTo(Product1Order));
            deadline = TimeProvider.Subtract(verticalOrder.ParentOrder.ShipmentDeadline,
                verticalOrder.ItemsOrdered.Single().TimeToProduce);
            Assert.That(verticalOrder.ShipmentDeadline, Is.EqualTo(deadline));
            Assert.That(verticalOrder.ShippedAt, Is.Null);
            Assert.That(verticalOrder.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(verticalOrder.Type, Is.EqualTo(OrderType.Internal));
            Assert.That(verticalOrder.Shop, Is.EqualTo(TimberComponentShop));
            Assert.That(verticalOrder.WorkStartedAt, Is.Null);
            Assert.That(verticalOrder.SubOrders, Is.Empty);

            var horizontalOrder = entities.Single(o => o.ItemsOrdered.Any(i => i.Article == Component2Horizontal));
            Assert.That(horizontalOrder.ItemsOrdered.Count, Is.EqualTo(1));
            Assert.That(horizontalOrder.ItemsProduced.Count, Is.EqualTo(1));
            var horizontalAmount = Product1InteriorDoor.PrimaryBillOfMaterial.Input.Of(Component2Horizontal).Amount *
                                   amount;
            Assert.That(horizontalOrder.ItemsOrdered.Of(Component2Horizontal).Amount, Is.EqualTo(horizontalAmount));
            Assert.That(horizontalOrder.ItemsProduced.Of(Component2Horizontal).Amount, Is.Zero);
            Assert.That(horizontalOrder.Customer, Is.EqualTo(Product1Order.Customer));
            Assert.That(horizontalOrder.ParentOrder, Is.EqualTo(doorsOrder));
            Assert.That(horizontalOrder.ReferenceOrder, Is.EqualTo(Product1Order));
            deadline = TimeProvider.Subtract(horizontalOrder.ParentOrder.ShipmentDeadline,
                horizontalOrder.ItemsOrdered.Single().TimeToProduce);
            Assert.That(horizontalOrder.ShipmentDeadline, Is.EqualTo(deadline));
            Assert.That(horizontalOrder.ShippedAt, Is.Null);
            Assert.That(horizontalOrder.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(horizontalOrder.Type, Is.EqualTo(OrderType.Internal));
            Assert.That(horizontalOrder.Shop, Is.EqualTo(TimberComponentShop));
            Assert.That(horizontalOrder.WorkStartedAt, Is.Null);
            Assert.That(horizontalOrder.SubOrders, Is.Empty);

            var mdfOrder = entities.Single(o => o.ItemsOrdered.Any(i => i.Article == Component3MdfFiller));
            Assert.That(mdfOrder.ItemsOrdered.Count, Is.EqualTo(1));
            Assert.That(mdfOrder.ItemsProduced.Count, Is.EqualTo(1));
            var mdfAmount = Product1InteriorDoor.PrimaryBillOfMaterial.Input.Of(Component3MdfFiller).Amount * amount;
            Assert.That(mdfOrder.ItemsOrdered.Of(Component3MdfFiller).Amount, Is.EqualTo(mdfAmount));
            Assert.That(mdfOrder.ItemsProduced.Of(Component3MdfFiller).Amount, Is.Zero);
            Assert.That(mdfOrder.Customer, Is.EqualTo(Product1Order.Customer));
            Assert.That(mdfOrder.ReferenceOrder, Is.EqualTo(Product1Order));
            Assert.That(mdfOrder.ParentOrder, Is.EqualTo(doorsOrder));
            deadline = TimeProvider.Subtract(mdfOrder.ParentOrder.ShipmentDeadline,
                mdfOrder.ItemsOrdered.Single().TimeToProduce);
            Assert.That(mdfOrder.ShipmentDeadline, Is.EqualTo(deadline));
            Assert.That(mdfOrder.ShippedAt, Is.Null);
            Assert.That(mdfOrder.Status, Is.EqualTo(OrderStatus.New));
            Assert.That(mdfOrder.Type, Is.EqualTo(OrderType.Internal));
            Assert.That(mdfOrder.Shop, Is.EqualTo(MdfComponentShop));
            Assert.That(mdfOrder.WorkStartedAt, Is.Null);
            Assert.That(mdfOrder.SubOrders, Is.Empty);

            Assert.That(GetRecordedEvents<DomainEvent<Order>>().Count, Is.EqualTo(4));

            //annul
            Product1Order.SubOrders = subOrdersInitial;
            ProductAssemblyShop.Orders = assemblyShopOrders;
            TimberComponentShop.Orders = timberShopOrders;
            MdfComponentShop.Orders = mdfShopOrders;
        }

        [Test]
        public async Task EnactOrder_Success()
        {
            //adhere
            var initialSuborder = Product1Order.SubOrders.DeepCopy();

            //arrange
            var input = Product1InteriorDoor.PrimaryBillOfMaterial.Input;
            var amount = Product1Order.ItemsOrdered.Of(Product1InteriorDoor).Amount;
            Product1Order.SubOrders = new HashSet<Order>()
            {
                new Order(Guid.NewGuid(), "Door suborder")
                {
                    Shop = ProductAssemblyShop,
                    ItemsOrdered = Product1Order.ItemsOrdered,
                    Status = OrderStatus.New,
                    ShipmentDeadline = Product1Order.ShipmentDeadline.AddDays(-1)
                },
                new Order(Guid.NewGuid(), "Vertical component suborder")
                {
                    Shop = TimberComponentShop,
                    ItemsOrdered = new HashSet<InventoryItem>
                    {
                        new InventoryItem(Component1Vertical, input.Of(Component1Vertical).Amount * amount)
                    },
                    Status = OrderStatus.New,
                    ShipmentDeadline = Product1Order.ShipmentDeadline.AddDays(-1)
                },
                new Order(Guid.NewGuid(), "Horizontal component suborder")
                {
                    Shop = TimberComponentShop,
                    ItemsOrdered = new HashSet<InventoryItem>
                    {
                        new InventoryItem(Component2Horizontal, input.Of(Component2Horizontal).Amount * amount)
                    },
                    Status = OrderStatus.New,
                    ShipmentDeadline = Product1Order.ShipmentDeadline.AddDays(-1)
                },
                new Order(Guid.NewGuid(), "Mdf component suborder")
                {
                    Shop = MdfComponentShop,
                    ItemsOrdered = new HashSet<InventoryItem>
                    {
                        new InventoryItem(Component3MdfFiller, input.Of(Component3MdfFiller).Amount * amount)
                    },
                    Status = OrderStatus.New,
                    ShipmentDeadline = Product1Order.ShipmentDeadline.AddDays(-1)
                }
            };
            var command = new EnactOrder
            {
                OrderId = Product1Order.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new EnactOrderHandler(OrderRepo, InventoryService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.True(Product1Order.SubOrders.All(o => o.Status == OrderStatus.InProduction));
            Assert.True(Product1Order.SubOrders.All(o => o.EstimatedCompletionAt != null));
            Assert.That(GetRecordedEvents<DomainEvent<Order>>().Count, Is.EqualTo(5));
            Assert.That(GetRecordedEvents<OrderEnacted>().Count, Is.EqualTo(1));

            //annul
            Product1Order.SubOrders = initialSuborder;
        }

        [Test]
        public async Task ShipOrder_Success()
        {
            //adhere
            var initialInventory = ProductStockpileShop.Inventory.DeepCopy();

            //arrange
            var command = new ShipOrder
            {
                Id = OrderReadyForShipment.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ShipOrderHandler(OrderRepo, OrderService, InventoryService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var orderAmount = OrderReadyForShipment.ItemsProduced.Of(Product1InteriorDoor).Amount;
            var inventoryDiff = initialInventory.Of(Product1InteriorDoor).Amount
                                - ProductStockpileShop.Inventory.Of(Product1InteriorDoor).Amount;
            Assert.That(orderAmount, Is.EqualTo(inventoryDiff));

            Assert.That(RecordedEventCount<OrderShipped>(), Is.EqualTo(1));
            Assert.That(RecordedEventCount<DomainEvent<Order>>(), Is.EqualTo(1));

            //annul
            ProductStockpileShop.Inventory = initialInventory;
        }
    }
}
