using System;
using System.Diagnostics;
using System.Linq;
using Epok.Core.Domain.Events;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Entities;
using NUnit.Framework;

namespace Epok.Integration.Tests.Crud
{
    [TestFixture]
    public class CustomersTests
    {

        [Test]
        public void CommandResponsibility()
        {
            //create
            var customerPost = Bogus.CustomerFaker.Generate();
            var id = WebApiClient.Customers.Post(customerPost);

            var customerGet = WebApiClient.Customers.Get(id);
            Assert.That(customerGet, Is.Not.Null);
            Assert.That(customerGet.Name, Is.EqualTo(customerPost.Name));
            Assert.That(customerGet.CustomerType, Is.EqualTo(customerPost.CustomerType));
            Assert.That(EqualityHolds.For(customerGet.ShippingAddress, customerPost.ShippingAddress));
            Assert.That(EqualityHolds.For(customerGet.PrimaryContact, customerPost.PrimaryContact));

            var domainEvent = RmqClient.Consume<DomainEvent<Customer>>();
            Assert.That(domainEvent, Is.Not.Null);
            Assert.That(domainEvent.Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(domainEvent.Entity.Id, Is.EqualTo(customerGet.Id));

            //update: customer type
            var newCustomerType = TestHelper.RandomEnumValue(excluding:customerPost.CustomerType);
            WebApiClient.Customers.PutCustomerType(id, newCustomerType);

            customerGet = WebApiClient.Customers.Get(id);
            Assert.That(customerGet.CustomerType, Is.EqualTo(newCustomerType));

            //ToDo:4 domainEvent = RmqClient.Consume<DomainEvent<Customer>>();
            //ToDo:4 Assert.That(domainEvent, Is.Not.Null);
            //ToDo:4 Assert.That(domainEvent.Trigger, Is.EqualTo(Trigger.Changed));
            //ToDo:4 Assert.That(domainEvent.Entity.Id, Is.EqualTo(customerGet.Id));

            //update: address
            var newAddress = Bogus.AddressFaker.Generate();
            WebApiClient.Customers.PutCustomerAddress(id, newAddress);

            customerGet = WebApiClient.Customers.Get(id);
            Assert.That(EqualityHolds.For(customerGet.ShippingAddress, newAddress));

            //ToDo:4 ContactsController/Address

            //update: contact post
            var newContact = Bogus.ContactFaker.Generate();
            var subId = WebApiClient.Customers.PostCustomerContact(id, newContact);
            
            customerGet = WebApiClient.Customers.Get(id);
            var contact = customerGet.Contacts.Single(c => c.Id == subId);
            Assert.That(EqualityHolds.For(contact, newContact));

            //update: contact put as primary
            WebApiClient.Customers.PutCustomerContactAsPrimary(id, subId);

            customerGet = WebApiClient.Customers.Get(id);
            Assert.That(customerGet.PrimaryContact.Id, Is.EqualTo(subId));

            //update: contact 
            var modifiedContact = Bogus.ContactFaker.Generate();
            WebApiClient.Customers.PutCustomerContact(id, subId, modifiedContact);

            customerGet = WebApiClient.Customers.Get(id);
            Assert.That(customerGet.PrimaryContact.Id, Is.EqualTo(subId));
            Assert.That(EqualityHolds.For(customerGet.PrimaryContact, modifiedContact));

            //delete contact
            var nonPrimaryContactId = customerGet.Contacts.Single(c => c.Primary == false).Id;
            WebApiClient.Customers.DeleteContact(id, nonPrimaryContactId);

            customerGet = WebApiClient.Customers.Get(id);
            var nonPrimaryContact = customerGet.Contacts.SingleOrDefault(c => c.Id == nonPrimaryContactId);
            Assert.That(nonPrimaryContact, Is.Null);

            //delete customer
            WebApiClient.Customers.Delete(id);

            customerGet = WebApiClient.Customers.Get(id);
            Assert.That(customerGet, Is.Null);

            //ToDo:4 domainEvent = RmqClient.Consume<DomainEvent<Customer>>();
            //ToDo:4 Assert.That(domainEvent, Is.Not.Null);
            //ToDo:4 Assert.That(domainEvent.Trigger, Is.EqualTo(Trigger.Removed));
        }

        [Test]
        public void QueryResponsibility()
        {
            Console.WriteLine("Query test started.");

            var customers = Bogus.CustomerFaker.Generate(1000);
            var testCountryIds = customers.Take(100).Select(c =>
            {
                c.ShippingAddress.Country = "TestCountry";
                return c.Id;
            }).ToHashSet();
            var testProvinceIds = customers.Skip(50).Take(100).Select(c =>
            {
                c.ShippingAddress.Province = "TestProvince";
                return c.Id;
            }).ToHashSet();
            var testCityIds = customers.Skip(100).Take(100).Select(c =>
            {
                c.ShippingAddress.City = "TestCity";
                return c.Id;
            }).ToHashSet();

            DbManager.BulkInsert(customers);

            Console.WriteLine("Db seeded.");

            var sw = new Stopwatch();
            sw.Start();
            var queried = WebApiClient.Customers.Get();
            sw.Stop();
            Assert.That(queried.Count(), Is.EqualTo(customers.Count));
            
            Console.WriteLine($"Query all completed: {customers.Count} in {sw.ElapsedMilliseconds} ms.");

            sw.Restart();
            queried = WebApiClient.Customers.Get(countryExact: "TestCountry");
            sw.Stop();
            Assert.That(testCountryIds, Is.Not.Empty);
            Assert.That(queried.Select(q => q.Id).ToHashSet().SetEquals(testCountryIds));
            
            Console.WriteLine($"Query and filter by country completed: {testCountryIds.Count} in {sw.ElapsedMilliseconds} ms.");

            sw.Restart();
            queried = WebApiClient.Customers.Get(provinceExact: "TestProvince");
            sw.Stop();
            Assert.That(testProvinceIds, Is.Not.Empty);
            Assert.That(queried.Select(q => q.Id).ToHashSet().SetEquals(testProvinceIds));

            Console.WriteLine($"Query and filter by province completed: {testProvinceIds.Count} in {sw.ElapsedMilliseconds} ms.");

            sw.Restart();
            queried = WebApiClient.Customers.Get(cityExact: "TestCity");
            sw.Stop();
            Assert.That(testCityIds, Is.Not.Empty);
            Assert.That(queried.Select(q => q.Id).ToHashSet().SetEquals(testCityIds));

            Console.WriteLine($"Query and filter by city completed: {testCityIds.Count} in {sw.ElapsedMilliseconds} ms.");
            
            var testProvinceCity = testProvinceIds.Intersect(testCityIds).ToHashSet();

            sw.Restart();
            queried = WebApiClient.Customers.Get(provinceExact: "TestProvince", cityExact: "TestCity");
            sw.Stop();
            Assert.That(testProvinceCity, Is.Not.Empty);
            Assert.That(queried.Select(q => q.Id).ToHashSet().SetEquals(testProvinceCity));

            Console.WriteLine($"Query and filter by province and city completed: {testProvinceCity.Count} in {sw.ElapsedMilliseconds} ms.");

            var customerType = TestHelper.RandomEnumValue(excluding: CustomerType.Undefined);
            var testTypeIds = customers.Where(c => c.CustomerType == customerType).Select(c => c.Id).ToHashSet();
            sw.Restart();
            queried = WebApiClient.Customers.Get(typeExact: customerType);
            sw.Stop();
            Assert.That(testTypeIds, Is.Not.Empty);
            Assert.That(queried.Select(q => q.Id).ToHashSet().SetEquals(testTypeIds));

            Console.WriteLine($"Query and filter by type completed: {testTypeIds.Count} in {sw.ElapsedMilliseconds} ms.");

        }
    }
}