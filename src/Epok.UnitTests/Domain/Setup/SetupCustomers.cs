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
            A.CallTo(() => CustomerRepo.LoadAsync(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);
            A.CallTo(() => CustomerRepo.GetAsync(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);

            A.CallTo(() => CustomerRepo.LoadAsync(CustomerToArchive.Id)).Returns(CustomerToArchive);
            A.CallTo(() => CustomerRepo.GetAsync(CustomerToArchive.Id)).Returns(CustomerToArchive);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<Customer>(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);
            A.CallTo(() => ReadOnlyRepo.GetAsync<Customer>(CustomerDoorsBuyer.Id)).Returns(CustomerDoorsBuyer);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<Customer>(CustomerToArchive.Id)).Returns(CustomerToArchive);
            A.CallTo(() => ReadOnlyRepo.GetAsync<Customer>(CustomerToArchive.Id)).Returns(CustomerToArchive);
        }
    }
}
