using Epok.Core.Domain.Entities;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Suppliers.Entities
{
    [Serializable]
    public class Supplier : EntityBase
    {
        public Supplier(Guid id, string name) : base(id, name)
        {
        }

        public HashSet<Article> SuppliableArticles { get; set; } = new HashSet<Article>();
        public HashSet<MaterialRequest> MaterialRequests { get; set; } = new HashSet<MaterialRequest>();
        public Address ShippingAddress { get; set; }
        public HashSet<Contact> Contacts { get; set; } = new HashSet<Contact>();
        public Contact PrimaryContact
            => Contacts.Single(c => c.Primary);
    }
}