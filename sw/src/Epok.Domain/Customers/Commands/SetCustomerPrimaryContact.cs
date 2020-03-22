using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Sets the primary contact of the customer.
    /// </summary>
    public class SetCustomerPrimaryContact : CommandBase
    {
        public Guid CustomerId { get; set; }
        public Guid NewPrimaryContactId { get; set; }
    }
}
