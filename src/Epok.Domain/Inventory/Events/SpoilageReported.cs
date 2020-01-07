using Epok.Core.Domain.Events;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System;

namespace Epok.Domain.Inventory.Events
{
    /// <summary>
    /// Event in response to spoilage reported.
    /// </summary>
    public class SpoilageReported : DomainEvent<Shop>
    {
        public SpoilageReported(Shop shop, InventoryItem spoilage, Guid userId)
            : base(shop, Trigger.Changed, userId)
        {
        }
    }
}
