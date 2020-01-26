using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Contacts.Entities
{
    /// <summary>
    /// Contains location data.
    /// </summary>
    [Serializable]
    public class Address : EntityBase
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public Address()
        {
        }
        public Address(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// House number, street.
        /// </summary>
        public virtual string AddressLine1 { get; set; }

        /// <summary>
        /// Apartment or office number.
        /// </summary>
        public virtual string AddressLine2 { get; set; }

        /// <summary>
        /// Settlement: city, town, village etc.
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Region: province, state, district etc.
        /// </summary>
        public virtual string Province { get; set; }

        /// <summary>
        /// State or territory.
        /// </summary>
        public virtual string Country { get; set; }

        /// <summary>
        /// Postal identifier: postal code, zip code etc.
        /// </summary>
        public virtual string PostalCode { get; set; }
       
        /// <summary>
        /// Customer or supplier.
        /// </summary>
        public virtual Guid CompanyId { get; set; }
    }
}
