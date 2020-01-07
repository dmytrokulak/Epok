using Epok.Core.Utilities;
using System;

namespace Epok.Domain.Contacts.Entities
{
    /// <summary>
    /// Contains basic contact information.
    /// </summary>
    [Serializable]
    public class Contact
    {
        public Contact(PersonName personName, string phoneNumber,
            string email, bool primary)
        {
            Guard.Against.InvalidEmail(email, nameof(email));
            Guard.Against.InvalidPhoneNumber(phoneNumber, nameof(phoneNumber));

            PersonName = personName;
            PhoneNumber = phoneNumber;
            Email = email;
            Primary = primary;
        }

        public PersonName PersonName { get; }
        public string PhoneNumber { get; }
        public string Email { get; }
        public bool Primary { get; }
    }
}
