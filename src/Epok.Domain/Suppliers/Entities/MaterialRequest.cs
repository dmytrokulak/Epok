using Epok.Core.Domain.Entities;
using Epok.Domain.Inventory.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Suppliers.Entities
{
    [Serializable]
    public class MaterialRequest : EntityBase
    {
        public MaterialRequest(Guid id, string name) : base(id, name)
        {
        }

        public virtual Supplier Supplier { get; set; }
        public virtual MaterialRequestStatus Status { get; set; }
        public virtual List<InventoryItem> ItemsRequested { get; set; }

        public virtual DateTimeOffset CreatedAt { get; set; }
    }
}
