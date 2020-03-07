using System;
using System.Collections.Generic;
using AutoMapper;
using Epok.Domain.Suppliers;
using Epok.Domain.Suppliers.Entities;
using Epok.Presentation.WebApi.Models.Inventory;

namespace Epok.Presentation.WebApi.Models.Suppliers
{
    public class MaterialRequestModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<MaterialRequest, MaterialRequestModel>();
            }
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MaterialRequestStatus Status { get; set; }
        public IEnumerable<InventoryItemModel> ItemsRequested { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
