using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;

namespace Epok.Presentation.WebApi.Models.Inventory
{
    public class RegisterArticleModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<RegisterArticleModel, RegisterArticle>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public ArticleType ArticleType { get; set; }
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }
        [Required]
        public Guid UomId { get; set; }
        public Guid? ProductionShopCategoryId { get; set; }
        public TimeSpan? TimeToProduce { get; set; }
        public IEnumerable<(Guid articleId, decimal amount)> BomInput { get; set; }
        public decimal BomOutput { get; set; }
    }
}
