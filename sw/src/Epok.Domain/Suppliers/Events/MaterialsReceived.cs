using Epok.Core.Domain.Events;
using Epok.Domain.Suppliers.Entities;
using System;

namespace Epok.Domain.Suppliers.Events
{
    /// <summary>
    /// Event raised in response of materials received
    /// under a material request.
    /// </summary>
    [Serializable]
    public class MaterialsReceived : DomainEvent<MaterialRequest>
    {
        public MaterialsReceived(MaterialRequest entity, Guid userId)
            : base(entity, Trigger.Changed, userId)
        {
        }
    }
}
