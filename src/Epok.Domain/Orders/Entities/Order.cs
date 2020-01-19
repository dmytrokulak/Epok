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

        public virtual Customer Customer { get; set; }

        public virtual HashSet<InventoryItem> ItemsOrdered { get; set; } = new HashSet<InventoryItem>();

        public virtual HashSet<InventoryItem> ItemsProduced { get; set; } = new HashSet<InventoryItem>();

        //should only be persisted when work is started
        public virtual DateTimeOffset? EstimatedCompletionAt { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset? WorkStartedAt { get; set; }
        public virtual DateTimeOffset? ShippedAt { get; set; }
        public virtual DateTimeOffset ShipmentDeadline { get; set; }

        /// <summary>
        /// Manufacturing orders for each shop calculated 
        /// according to bills of material of items ordered.
        /// </summary>
        public virtual HashSet<Order> SubOrders { get; set; } = new HashSet<Order>();

        public virtual Order ParentOrder { get; set; }

        //External order
        public virtual Order ReferenceOrder { get; set; }
        public virtual OrderStatus Status { get; set; }

        public virtual OrderType Type { get; set; }

        public virtual Shop Shop { get; set; }
    }
}
