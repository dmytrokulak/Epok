using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Contacts.Commands;
using Epok.Domain.Customers.Commands;

namespace Epok.Presentation.Model.Customers
{
    public class ContactModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ContactModel, RegisterContact>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.Name, m => m.Ignore())
                    .ForMember(m => m.CompanyId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());

                CreateMap<ContactModel, ChangeCustomerContact>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.CustomerId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] [MaxLength(30)] public string FirstName { get; set; }
        [MaxLength(30)] public string LastName { get; set; }
        [Required] [Phone] public string PhoneNumber { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
    }
}
