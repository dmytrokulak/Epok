using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;

namespace Epok.Presentation.Model.Customers
{
    public class ChangeCustomerTypeModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ChangeCustomerTypeModel, ChangeCustomerType>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] public CustomerType NewCustomerType { get; set; }
    }
}
