using Bogus;
using Epok.Core.Utilities;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Entities;
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
    public static class CustomerFaker
    {
        public static Customer Generate()
        {
            return Generate(1)[0];
        }
        public static IList<Customer> Generate(int amount)
        {
            var addresses = AddressFaker.Generate(amount);
            var contacts = ContactFaker.Generate(amount);

            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.CustomerType, f => f.PickRandomWithout(CustomerType.Undefined));
            var customers = customerFaker.Generate(amount);

            for (var i = 0; i < customers.Count; i++)
            {
                customers[i].Contacts = contacts[i].CollectToHashSet();
                contacts[i].CompanyId = customers[i].Id;
                customers[i].ShippingAddress = addresses[i];
                addresses[i].CompanyId = customers[i].Id;
            }

            return customers;
        }
    }
}
