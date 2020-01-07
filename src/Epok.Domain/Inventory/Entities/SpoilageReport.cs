using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Report on inventory written down due to
    /// manufacturing defects
    /// </summary>
    [Serializable]
    public class SpoilageReport : EntityBase
    {
        public SpoilageReport(Guid id, string name) : base(id, name)
        {
        }

        public InventoryItem Item { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset TimeOfReport { get; set; } = DateTimeOffset.Now;
    }
}
