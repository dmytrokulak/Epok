using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Inventory.Commands;

namespace Epok.Presentation.Model.Inventory
{
    public class ReportSpoilageModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ReportSpoilageModel, ReportSpoilage>()
                    .ForMember(m => m.ArticleId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] public Guid ShopId { get; set; }
        [Required] public Guid ArticleId { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public Guid OrderId { get; set; }
        [Required] public bool Fixable { get; set; }
        [Required] public string Reason { get; set; }
    }
}
