using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;

namespace Epok.Presentation.Model.Customers
{
    /// <summary>
    /// Registers new customer with the system.
    /// </summary>
    public class RegisterCustomerModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<RegisterCustomerModel, RegisterCustomer>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] [MaxLength(50)] public string Name { get; set; }
        [Required] public CustomerType Type { get; set; }
        [Required] [MaxLength(30)] public string PrimaryContactFirstName { get; set; }
        [MaxLength(30)] public string PrimaryContactLastName { get; set; }
        [Required] [Phone] public string PrimaryContactPhone { get; set; }
        [Required] [EmailAddress] public string PrimaryContactEmail { get; set; }
        [Required] [MaxLength(50)] public string ShippingAddressLine1 { get; set; }
        [MaxLength(50)] public string ShippingAddressLine2 { get; set; }
        [Required] [MaxLength(50)] public string ShippingAddressCity { get; set; }
        [MaxLength(50)] public string ShippingAddressProvince { get; set; }
        [MaxLength(50)] public string ShippingAddressCountry { get; set; }
        [MaxLength(10)] public string ShippingAddressPostalCode { get; set; }
    }
}
