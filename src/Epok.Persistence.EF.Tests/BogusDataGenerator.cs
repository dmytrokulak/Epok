using Bogus;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Persistence.EF.Tests
{
    /// <summary>
    /// Generates entities using Bogus library.
    /// </summary>
    /// <remarks>
    /// https://github.com/bchavez/Bogus
    /// </remarks>
    public class BogusDataGenerator
    {
        public static IList<Customer> GetCustomers(int amount)
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
            var addresses = addressFaker.Generate(amount);

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
            var contacts = contactFaker.Generate(amount);

            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.CustomerType, f => f.PickRandom<CustomerType>());
            var customers = customerFaker.Generate(amount);

            for (var i = 0; i < customers.Count; i++)
            {
                customers[i].Contacts = contacts[i].CollectToHashSet();
                contacts[i].CompanyId = customers[i].Id;
                customers[i].ShippingAddress = addresses[i];
            }

            return customers;
        }
    }
}
