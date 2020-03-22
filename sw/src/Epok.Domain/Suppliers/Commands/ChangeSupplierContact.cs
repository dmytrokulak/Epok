using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Modifies properties of the contact
    /// (except for "Primary" property).
    /// </summary>
    public class ChangeSupplierContact : CommandBase
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
