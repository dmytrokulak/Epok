using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Modifies properties of the contact
    /// (except for "Primary" property).
    /// </summary>
    public class ChangeCustomerContact : CommandBase
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
