using System;
using Epok.Core.Domain.Entities;

namespace Epok.Domain.Contacts.Entities
{
    /// <summary>
    /// Contains basic contact information.
    /// </summary>
    [Serializable]
    public class Contact : EntityBase
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public Contact()
        {
        }

        public Contact(Guid id, string name) : base(id, name)
        {
        }

        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Primary { get; set; }

        /// <summary>
        /// Customer or supplier.
        /// </summary>
        public virtual Guid CompanyId { get; set; }
    }
}
