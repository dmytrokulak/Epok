using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Inventory.Commands;

namespace Epok.Presentation.WebApi.Models.Inventory
{
    public class TransferInventoryModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<TransferInventoryModel, TransferInventory>()
                    .ForMember(m => m.ArticleId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] public Guid ArticleId { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public Guid SourceShopId { get; set; }
        [Required] public Guid TargetShopId { get; set; }
    }
}
