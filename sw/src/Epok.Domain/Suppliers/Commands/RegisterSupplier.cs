using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Registers a supplier in the system.
    /// </summary>
    public class RegisterSupplier : CreateEntityCommand
    {
        public IEnumerable<Guid> SuppliableArticleIds { get; set; }
        public string PrimaryContactFirstName { get; set; }
        public string PrimaryContactLastName { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressProvince { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string ShippingAddressPostalCode { get; set; }
    }
}
