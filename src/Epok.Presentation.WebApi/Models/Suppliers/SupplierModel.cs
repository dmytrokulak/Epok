using System;
using System.Collections.Generic;
using AutoMapper;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Presentation.WebApi.Models.Suppliers
{
    public class SupplierModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Supplier, SupplierModel>();
            }
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<EntityModel> SuppliableArticles { get; set; }
        public IEnumerable<EntityModel> MaterialRequests { get; set; }
        public Address ShippingAddress { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public Contact PrimaryContact { get; set; }
    }
}
