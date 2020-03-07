﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Epok.Domain.Suppliers.Commands;
using Epok.Domain.Suppliers.Entities;
using Epok.Presentation.WebApi.Models.Inventory;

namespace Epok.Presentation.WebApi.Models.Suppliers
{
    /// <summary>
    /// Creates a new material request with the supplier.
    /// Domain exception is thrown if the supplier does
    /// not supplier the articles requested.
    /// </summary>
    public class CreateMaterialRequestModel 
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateMaterialRequestModel, CreateMaterialRequest>()
                    .ForMember(r => r.Id, m => m.Ignore())
                    .ForMember(r => r.Name, m => m.Ignore())
                    .ForMember(r => r.SupplierId, m => m.Ignore())
                    .ForMember(r => r.InitiatorId, m => m.Ignore())
                    ;
            }
        }
        public IEnumerable<InventoryItemLightModel> Items { get; set; }
    }
}
