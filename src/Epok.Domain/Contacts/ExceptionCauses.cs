using Epok.Domain.Contacts.Entities;

namespace Epok.Domain.Contacts
{
    public static class ExceptionCauses
    {
        public static string ArchivingPrimaryContact(Contact contact)
            => $"Cannot archive {contact} because: primary for company {contact.CompanyId}.";
    }
}
