using Bogus;
using Epok.Domain.Contacts.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Bogus
{
    /// <summary>
    /// Generates entities using Bogus library.
    /// </summary>
    /// <remarks>
    /// https://github.com/bchavez/Bogus
    /// </remarks>
    public static class AddressFaker
    {
        public static Address Generate()
        {
            return Generate(1)[0];
        }
        public static IList<Address> Generate(int amount)
        {
            var addressFaker = new Faker<Address>()
                .RuleFor(a => a.Id, f => Guid.NewGuid())
                .RuleFor(a => a.AddressLine1, f => f.Address.StreetAddress())
                .RuleFor(a => a.AddressLine2, f => f.Address.SecondaryAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Province, f => f.Address.County())
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Name, (f, a) => $"{a.PostalCode}, {a.AddressLine1}, {a.City}, {a.Country}")
                .RuleFor(c => c.CompanyId, f => Guid.Empty)
                .StrictMode(true);
            return addressFaker.Generate(amount);
        }
    }
}
