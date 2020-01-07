using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System;

namespace Epok.Domain.Inventory.Events
{
    /// <summary>
    /// Event in response to an inventory item transferred.
    /// </summary>
    public class InventoryTransferred : DomainEvent<InventoryItem>
    {
        public InventoryTransferred(Shop sourceShop, Shop targetShop,
            InventoryItem inventoryItem, Guid userId) : base(inventoryItem, Trigger.Changed, userId)
        {
            SourceShop = sourceShop;
            TargetShop = targetShop;
            InventoryItem = inventoryItem;
        }

        public Shop SourceShop { get; }
        public Shop TargetShop { get; }
        public InventoryItem InventoryItem { get; }
    }
}
