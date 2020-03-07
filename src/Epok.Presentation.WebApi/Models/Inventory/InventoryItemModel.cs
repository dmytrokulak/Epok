using System;
using AutoMapper;
using Epok.Domain.Inventory.Entities;

namespace Epok.Presentation.WebApi.Models.Inventory
{
    public class InventoryItemModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryItem, InventoryItemModel>();
            }
        }

        public EntityModel Article { get; set; }
        public decimal Amount { get; set; }
    }

    public class InventoryItemLightModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<InventoryItemLightModel, (Guid, decimal)>()
                    .ForMember(t => t.Item1, m => m.MapFrom(i => i.ArticleId))
                    .ForMember(t => t.Item2, m => m.MapFrom(i => i.Amount));
            }
        }

        public Guid ArticleId { get; set; }
        public decimal Amount { get; set; }
    }
}
