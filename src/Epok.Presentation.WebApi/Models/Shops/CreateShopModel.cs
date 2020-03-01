using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Shops.Commands;

namespace Epok.Presentation.WebApi.Models.Shops
{
    /// <summary>
    /// Creates new shop and assigns a manager.
    /// </summary>
    public class CreateShopModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateShopModel, CreateShop>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        /// <summary>
        /// Shop name
        /// </summary>
       [Required] public string Name { get; set; }

        /// <summary>
        /// ShopCategoryId
        /// </summary>
        [Required] public Guid? ShopCategoryId { get; set; }

        /// <summary>
        /// Id of a user to be set as a manager.
        /// </summary>
        [Required] public Guid? ManagerId { get; set; }

        /// <summary>
        /// Whether materials can be supplied 
        /// to this shop from an external supplier.
        /// </summary>
        public bool IsEntryPoint { get; set; }

        /// <summary>
        /// Whether products can be shipped
        /// to customers from this shop.
        /// </summary>
        public bool IsExitPoint { get; set; }

    }
}
