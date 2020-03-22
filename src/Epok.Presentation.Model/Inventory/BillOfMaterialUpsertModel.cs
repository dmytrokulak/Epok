using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Inventory.Commands;

namespace Epok.Presentation.Model.Inventory
{
    public class BillOfMaterialUpsertModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<BillOfMaterialUpsertModel, AddBillOfMaterial>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.Name, m => m.Ignore())
                    .ForMember(m => m.ArticleId, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());

                CreateMap<BillOfMaterialUpsertModel, ChangeBillOfMaterial>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required]
        public IEnumerable<(Guid articleId, decimal amount)> Input { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal Output { get; set; }
    }
}
