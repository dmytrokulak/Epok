using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Sets the primary contact of the customer.
    /// </summary>
    public class SetSupplierPrimaryContact : CommandBase
    {
        public Guid SupplierId { get; set; }
        public Guid NewPrimaryContactId { get; set; }
    }
}
