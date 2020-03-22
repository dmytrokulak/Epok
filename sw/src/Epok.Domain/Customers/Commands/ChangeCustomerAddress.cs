using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Changes customer's shipping address.
    /// </summary>
    public class ChangeCustomerAddress : CommandBase
    {
        public Guid Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
