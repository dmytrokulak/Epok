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

        public virtual HashSet<Article> SuppliableArticles { get; set; } = new HashSet<Article>();
        public virtual HashSet<MaterialRequest> MaterialRequests { get; set; } = new HashSet<MaterialRequest>();
        public virtual Address ShippingAddress { get; set; }
        public virtual HashSet<Contact> Contacts { get; set; } = new HashSet<Contact>();
        public virtual Contact PrimaryContact
            => Contacts.Single(c => c.Primary);
    }
}