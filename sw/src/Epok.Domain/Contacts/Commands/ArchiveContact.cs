using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Contacts.Commands
{
    /// <summary>
    /// Removes a contact. Throws an exception if the contact
    /// is a primary contact for a company.
    /// </summary>
    public class ArchiveContact : ArchiveEntityCommand
    {
        public Guid CompanyId { get; set; }
    }
}
