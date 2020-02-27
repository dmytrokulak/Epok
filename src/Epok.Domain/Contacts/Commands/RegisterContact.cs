using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Contacts.Commands
{
    /// <summary>
    /// Adds new contact to the collection of customer's/supplier's contacts.
    /// </summary>
    public class RegisterContact : CommandBase
    {
        public Guid CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
