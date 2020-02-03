using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System;

namespace Epok.Domain.Inventory.Events
{
    /// <summary>
    /// Event in response to an inventory item produced.
    /// </summary>
    [Serializable]
    public class InventoryItemProduced : DomainEvent<Shop>
    {
        public InventoryItem Produced { get; }
        public Order Order { get; }

        public InventoryItemProduced(Shop shop, InventoryItem produced, Order order, Guid userId)
            : base(shop, Trigger.Changed, userId)
        {
            Produced = produced;
            Order = order;
        }
    }
}