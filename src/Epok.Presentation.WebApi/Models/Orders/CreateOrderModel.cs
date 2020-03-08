using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Epok.Domain.Orders;
using Epok.Domain.Orders.Commands;
using Epok.Presentation.WebApi.Models.Inventory;

namespace Epok.Presentation.WebApi.Models.Orders
{
    public class CreateOrderModel
    {
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<CreateOrderModel, CreateOrder>()
                    .ForMember(m => m.Id, m => m.Ignore())
                    .ForMember(m => m.Name, m => m.Ignore())
                    .ForMember(m => m.InitiatorId, m => m.Ignore());
            }
        }

        [Required] public Guid CustomerId { get; set; }
        [Required] public IEnumerable<InventoryItemLightModel> Items { get; set; }
        [Required] public DateTimeOffset ShipmentDeadline { get; set; }
        [Required] public OrderType OrderType { get; set; }
    }
}
