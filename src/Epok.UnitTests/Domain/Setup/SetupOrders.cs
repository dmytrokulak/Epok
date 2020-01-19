using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders;
using Epok.Domain.Orders.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.UnitTests.Domain.Setup
{
    public abstract partial class SetupBase
    {
        protected Order Product1Order = new Order(Guid.NewGuid(), "Finished interior door order");
        protected Order Product1OrderFulfilled = new Order(Guid.NewGuid(), "Finished interior door order full");
        protected Order OrderReadyForShipment = new Order(Guid.NewGuid(), "Order ready for shipment");
        protected HashSet<Order> Orders;

        private void InitOrders()
        {

            Product1Order.ItemsOrdered = new HashSet<InventoryItem>
            {
                new InventoryItem(Product1InteriorDoor, 20)
            };
            Product1Order.ItemsProduced = new HashSet<InventoryItem>
            {
                InventoryItem.Empty(Product1InteriorDoor)
            };
            Product1Order.ShipmentDeadline = DateTimeOffset.Parse("2022-01-01");
            Product1Order.Customer = CustomerDoorsBuyer;

            Product1OrderFulfilled.ItemsOrdered = new HashSet<InventoryItem>
            {
                new InventoryItem(Product1InteriorDoor, 20)
            };
            Product1OrderFulfilled.ItemsProduced = new HashSet<InventoryItem>
            {
                new InventoryItem(Product1InteriorDoor, 20)
            };
            Product1OrderFulfilled.ShipmentDeadline = DateTimeOffset.Parse("2020-01-01");
            Product1OrderFulfilled.Customer = CustomerDoorsBuyer;

            OrderReadyForShipment.ItemsOrdered = new HashSet<InventoryItem>
            {
                new InventoryItem(Product1InteriorDoor, 20)
            };
            OrderReadyForShipment.ItemsProduced = new HashSet<InventoryItem>
            {
                new InventoryItem(Product1InteriorDoor, 20)
            };
            OrderReadyForShipment.ShipmentDeadline = DateTimeOffset.Parse("2020-01-01");
            OrderReadyForShipment.Status = OrderStatus.ReadyForShipment;

            Orders = new HashSet<Order> {Product1Order, Product1OrderFulfilled, OrderReadyForShipment};
        }

        private void StubOrdersRepositories()
        {
            A.CallTo(() => OrderRepo.LoadAsync(Product1Order.Id)).Returns(Product1Order);
            A.CallTo(() => OrderRepo.GetAsync(Product1Order.Id)).Returns(Product1Order);

            A.CallTo(() => OrderRepo.LoadAsync(Product1OrderFulfilled.Id)).Returns(Product1OrderFulfilled);
            A.CallTo(() => OrderRepo.GetAsync(Product1OrderFulfilled.Id)).Returns(Product1OrderFulfilled);

            A.CallTo(() => OrderRepo.LoadAsync(OrderReadyForShipment.Id)).Returns(OrderReadyForShipment);
            A.CallTo(() => OrderRepo.GetAsync(OrderReadyForShipment.Id)).Returns(OrderReadyForShipment);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<Order>(Product1Order.Id)).Returns(Product1Order);
            A.CallTo(() => ReadOnlyRepo.GetAsync<Order>(Product1Order.Id)).Returns(Product1Order);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<Order>(Product1OrderFulfilled.Id)).Returns(Product1OrderFulfilled);
            A.CallTo(() => ReadOnlyRepo.GetAsync<Order>(Product1OrderFulfilled.Id)).Returns(Product1OrderFulfilled);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<Order>(OrderReadyForShipment.Id)).Returns(OrderReadyForShipment);
            A.CallTo(() => ReadOnlyRepo.GetAsync<Order>(OrderReadyForShipment.Id)).Returns(OrderReadyForShipment);

            A.CallTo(() => OrderRepo.GetSomeAsync(A<IEnumerable<Guid>>.That.Matches(x => x.Single() == Product1Order.Id)))
                .Returns(Product1Order.Collect().ToList());
        }
    }
}
