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
        public Address(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// House number, street.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Apartment or office number.
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Settlement: city, town, village etc.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Region: provice, state, district etc.
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// State or territory.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Postal identifier: postal code, zip code etc.
        /// </summary>
        public string PostalCode { get; set; }
    }
}
