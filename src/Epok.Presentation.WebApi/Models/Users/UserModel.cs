using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Users.Commands;

namespace Epok.Presentation.WebApi.Models.Users
{
    /// <summary>
    /// Corresponds to a user entity.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Mapping from model to command.
        /// </summary>
        public class MappingProfile : Profile
        {
            /// <summary>
            /// Mapping from model to command.
            /// </summary>
            public MappingProfile()
            {
                CreateMap<UserModel, CreateUser>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.Name, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());

                CreateMap<UserModel, ChangeUserData>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.Name, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        /// <summary>
        /// First (given) name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last (family) name.
        /// </summary>

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
