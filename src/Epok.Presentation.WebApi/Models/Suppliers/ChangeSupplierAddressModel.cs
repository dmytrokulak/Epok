using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Suppliers.Commands;

namespace Epok.Presentation.WebApi.Models.Suppliers
{
    public class ChangeSupplierAddressModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ChangeSupplierAddressModel, ChangeSupplierAddress>()
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
