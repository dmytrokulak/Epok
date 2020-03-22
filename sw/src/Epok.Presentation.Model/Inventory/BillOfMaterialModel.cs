using System.Collections.Generic;
using AutoMapper;
using Epok.Domain.Inventory.Entities;

namespace Epok.Presentation.Model.Inventory
{
    public class BillOfMaterialModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<BillOfMaterial, BillOfMaterialModel>();
            }
        }

        public IEnumerable<EntityModel> Input { get; set; }
        public decimal Output { get; set; }
        public bool Primary { get; set; }
    }
}
