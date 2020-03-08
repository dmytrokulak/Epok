using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Customers.Commands;

namespace Epok.Presentation.WebApi.Models.Customers
{
    public class ChangeCustomerAddressModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ChangeCustomerAddressModel, ChangeCustomerAddress>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }
        [Required] [MaxLength(50)] public string AddressLine1 { get; set; }
        [Required] [MaxLength(50)] public string AddressLine2 { get; set; }
        [Required] [MaxLength(50)] public string City { get; set; }
        [Required] [MaxLength(50)] public string Province { get; set; }
        [Required] [MaxLength(50)] public string Country { get; set; }
        [Required] [MaxLength(10)] public string PostalCode { get; set; }
    }
}
