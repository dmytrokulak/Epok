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

        public Supplier Supplier { get; set; }
        public MaterialRequestStatus Status { get; set; }
        public List<InventoryItem> ItemsRequested { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
