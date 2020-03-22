using System;
using AutoMapper;
using Epok.Domain.Inventory.Entities;

namespace Epok.Presentation.Model.Inventory
{
    public class ArticleModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Article, ArticleModel>();
            }
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ArticleType { get; set; }

        public string Code { get; set; }
        public EntityModel UoM { get; set; }

        public EntityModel ProductionShopCategory { get; set; }
        public EntityModel PrimaryBillOfMaterial { get; set; }
    }
}
