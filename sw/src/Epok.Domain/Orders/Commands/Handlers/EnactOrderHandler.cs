﻿using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Events;
using System;
using System.Threading.Tasks;
using static Epok.Domain.Orders.ExceptionCauses;

namespace Epok.Domain.Orders.Commands.Handlers
{
    /// <summary>
    /// Passes order to workshops for manufacturing.
    /// </summary>
    public class EnactOrderHandler : ICommandHandler<EnactOrder>
    {
        private readonly IEntityRepository _repository;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public EnactOrderHandler(IEntityRepository repository, IInventoryService inventoryService,
            IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(EnactOrder command)
        {
            var order = await _repository.GetAsync<Order>(command.OrderId);

            foreach (var subOrder in order.SubOrders)
            {
                subOrder.Status = OrderStatus.InProduction;
                subOrder.WorkStartedAt = DateTimeOffset.Now;
                var eta = await _inventoryService.CalculateTimeOfCompletion(subOrder.ItemsOrdered);
                if (eta > subOrder.ShipmentDeadline)
                    throw new DomainException(InsufficientTime(subOrder.Name, subOrder.ShipmentDeadline, eta));
                subOrder.EstimatedCompletionAt = eta;
            }

            foreach (var subOrder in order.SubOrders)
                await _eventTransmitter.BroadcastAsync(new DomainEvent<Order>(subOrder, Trigger.Added,
                    command.InitiatorId));

            await _eventTransmitter.BroadcastAsync(new OrderEnacted(order, command.InitiatorId));
        }
    }
}
