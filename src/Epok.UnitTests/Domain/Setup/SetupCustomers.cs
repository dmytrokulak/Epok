using Epok.Domain.Customers.Entities;
using Epok.Domain.Orders.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;

namespace Epok.UnitTests.Domain.Setup
{
    public abstract partial class SetupBase
    {
        protected Customer CustomerDoorsBuyer = new Customer(Guid.NewGuid(), "Doors buyer.");
        protected Customer CustomerToArchive = new Customer(Guid.NewGuid(), "Customer to archive");

        private void InitCustomers()
        {
            CustomerDoorsBuyer.Orders = new HashSet<Order> {Product1Order, Product1OrderFulfilled};
        }

        private void StubCustomersRepositories()
        {
            A.CallTo(() => EntityRepository.LoadAsync<Customer>(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);
            A.CallTo(() => EntityRepository.GetAsync<Customer>(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);

            A.CallTo(() => EntityRepository.LoadAsync<Customer>(CustomerToArchive.Id)).Returns(CustomerToArchive);
            A.CallTo(() => EntityRepository.GetAsync<Customer>(CustomerToArchive.Id)).Returns(CustomerToArchive);
        }
    }
}
