using Epok.Core.Domain.Entities;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Orders.Entities
{
    [Serializable]
    public class Order : EntityBase
    {
        public Order(Guid id, string name) : base(id, name)
        {
        }

        public Customer Customer { get; set; }

        public HashSet<InventoryItem> ItemsOrdered { get; set; } = new HashSet<InventoryItem>();

        public HashSet<InventoryItem> ItemsProduced { get; set; } = new HashSet<InventoryItem>();

        //should only be persisted when work is started
        public DateTimeOffset? EstimatedCompletionAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? WorkStartedAt { get; set; }
        public DateTimeOffset? ShippedAt { get; set; }
        public DateTimeOffset ShipmentDeadline { get; set; }

        /// <summary>
        /// Manufacturing orders for each shop calculated 
        /// according to bills of material of items ordered.
        /// </summary>
        public HashSet<Order> SubOrders { get; set; } = new HashSet<Order>();

        public Order ParentOrder { get; set; }

        //External order
        public Order ReferenceOrder { get; set; }
        public OrderStatus Status { get; set; }

        public OrderType Type { get; set; }

        public Shop Shop { get; set; }
    }
}
