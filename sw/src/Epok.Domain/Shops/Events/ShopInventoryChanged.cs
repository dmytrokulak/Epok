using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Shops.Events
{
    /// <summary>
    /// Event raised in response to a shop inventory
    /// increase or decrease.
    /// </summary>
    [Serializable]
    public class ShopInventoryChanged : DomainEvent<Shop>
    {
        public ShopInventoryChanged(Shop shop, IEnumerable<InventoryItem> newInventory, Guid userId)
            : base(shop, Trigger.Changed, userId)
        {
            Shop = shop;
            NewInventory = newInventory;
        }

        public Shop Shop { get; }
        public IEnumerable<InventoryItem> NewInventory { get; }
    }
}
