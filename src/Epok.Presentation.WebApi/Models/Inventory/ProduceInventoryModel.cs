using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Inventory.Commands;

namespace Epok.Presentation.WebApi.Models.Inventory
{
    public class ProduceInventoryModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ProduceInventoryModel, ProduceInventoryItem>()
                    .ForMember(m => m.ArticleId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] public Guid ShopId { get; set; }
        [Required] public Guid ArticleId { get; set; }
        //ToDo:3 [NotLessThanZero]
        [Required] public decimal Amount { get; set; }
        [Required] public Guid OrderId { get; set; }
    }
}
