using System;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using Epok.Domain.Customers.Entities;
using Epok.Presentation.Model.Customers;
using Epok.Domain.Contacts.Entities;

namespace Epok.Integration.Tests
{
    internal static class EpokMapper
    {
        private static IMapper _instance;

        internal static TDestination Map<TDestination>(object source) 
            => (_instance ??= GetMapper()).Map<TDestination>(source);

        private static IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();

            var epokAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Epok"));

            mce.AddMaps(epokAssemblies);

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc);
        }
    }
    internal class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Customer, RegisterCustomerModel> ()
                .ForMember(m => m.Type, m => m.MapFrom(c => c.CustomerType))
                .ForMember(m => m.PrimaryContactPhone, m => m.MapFrom(c => c.PrimaryContact.PhoneNumber))
                .ForMember(m => m.ShippingAddressLine1, m => m.MapFrom(c => c.ShippingAddress.AddressLine1))
                .ForMember(m => m.ShippingAddressLine2, m => m.MapFrom(c => c.ShippingAddress.AddressLine2));

            CreateMap<Address, ChangeCustomerAddressModel>();
            CreateMap<Contact, ContactModel>();
        }
    }
}
