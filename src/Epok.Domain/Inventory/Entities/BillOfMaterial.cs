using Epok.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Contains calculations on how many article
    /// units are required to produce a specified
    /// amount some article e.g.
    /// how much material is required to produce 
    /// a piece of product.
    /// </summary>
    [Serializable]
    public class BillOfMaterial : EntityBase
    {
        public BillOfMaterial(Guid id, string name) : base(id, name)
        {
        }

        public Article Article { get; set; }
        public HashSet<InventoryItem> Input { get; set; }
        public decimal Output { get; set; }
        public bool Primary { get; set; }
    }
}
