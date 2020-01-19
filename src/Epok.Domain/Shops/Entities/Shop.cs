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

        public virtual ShopCategory ShopCategory { get; set; }
        public virtual bool IsDefaultForCategory { get; set; }
        public virtual bool IsEntryPoint { get; set; }
        public virtual bool IsExitPoint { get; set; }
        public virtual User Manager { get; set; }
        public virtual HashSet<InventoryItem> Inventory { get; set; } = new HashSet<InventoryItem>();
        public virtual HashSet<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
