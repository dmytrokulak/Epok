using Epok.Core.Domain.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Users.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Shops.Entities
{
    /// <summary>
    /// Defines a physical space where
    /// inventory items can be stored
    /// and processed, be it a workshop
    /// or a warehouse.
    /// </summary>
    [Serializable]
    public class Shop : EntityBase
    {
        public Shop(Guid id, string name) : base(id, name)
        {
        }

        public ShopCategory ShopCategory { get; set; }
        public bool IsDefaultForCategory { get; set; }
        public bool IsEntryPoint { get; set; }
        public bool IsExitPoint { get; set; }
        public User Manager { get; set; }
        public HashSet<InventoryItem> Inventory { get; set; } = new HashSet<InventoryItem>();
        public HashSet<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
