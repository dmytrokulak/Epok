using Epok.Core.Domain.Entities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Customers.Entities
{
    /// <summary>
    /// Consignee of the finished products.
    /// Submits orders and receives the goods shipped.
    /// </summary>
    [Serializable]
    public class Customer : EntityBase
    {
        public Customer(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// All orders ever submitted by the customer.
        /// </summary>
        public virtual HashSet<Order> Orders { get; set; } = new HashSet<Order>();

        /// <summary>
        /// According to the scale of business: single retailer, 
        /// a retail network, a distributor etc.
        /// </summary>
        public virtual CustomerType CustomerType { get; set; }

        public virtual Address ShippingAddress { get; set; }
        public virtual HashSet<Contact> Contacts { get; set; } = new HashSet<Contact>();

        public virtual Contact PrimaryContact
            => Contacts.Single(c => c.Primary);
    }
}
