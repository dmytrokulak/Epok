using Epok.Domain.Contacts.Entities;
using NUnit.Framework;

namespace Epok.Integration.Tests
{
    public static class EqualityHolds
    {
        public static bool For(Address x, Address y)
        {
            Assert.That(x.AddressLine1, Is.EqualTo(y.AddressLine1));
            Assert.That(x.AddressLine2, Is.EqualTo(y.AddressLine2));
            Assert.That(x.City, Is.EqualTo(y.City));
            Assert.That(x.Province, Is.EqualTo(y.Province));
            Assert.That(x.Country, Is.EqualTo(y.Country));
            Assert.That(x.PostalCode, Is.EqualTo(y.PostalCode));
            return true;
        }

        public static bool For(Contact x, Contact y)
        {
            Assert.That(x.FirstName, Is.EqualTo(y.FirstName));
            Assert.That(x.LastName, Is.EqualTo(y.LastName));
            Assert.That(x.PhoneNumber, Is.EqualTo(y.PhoneNumber));
            Assert.That(x.Email, Is.EqualTo(y.Email));
            return true;
        }
    }
}
