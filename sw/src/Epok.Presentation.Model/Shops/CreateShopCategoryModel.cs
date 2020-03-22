using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Shops;
using Epok.Domain.Shops.Commands;

namespace Epok.Presentation.Model.Shops
{
    /// <summary>
    /// Creates a new shop category.
    /// </summary>
    public class CreateShopCategoryModel
    { 
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateShopCategoryModel, CreateShopCategory>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        /// <summary>
        /// ShopCategory name.
        /// </summary>
        [Required] public string Name { get; set; }
        /// <summary>
        /// Workshop or warehouse.
        /// </summary>
        [Required] public ShopType? ShopType { get; set; }

        /// <summary>
        /// Articles allowed in the shop.
        /// </summary>
        public IEnumerable<Guid> Articles { get; set; }
    }
}
