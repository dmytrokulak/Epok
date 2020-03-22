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
    public static class ContactFaker
    {
        public static Contact Generate()
        {
            return Generate(1)[0];
        }
        public static IList<Contact> Generate(int amount)
        {
            var contactFaker = new Faker<Contact>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, (f, c) => $"Contact {c.Id}")
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.MiddleName, f => "")
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Primary, f => true)
                .RuleFor(c => c.CompanyId, f => Guid.Empty)
                .StrictMode(true);
            return contactFaker.Generate(amount);
        }
    }
}
