﻿using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Services;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Produces an inventory item according to the article's bill of material.
    /// </summary>
    /// <exception cref="DomainException">
    /// Throw if materials are lacking for production, order does not have
    /// provision for the amount requested to be produced or the shop's category
    /// does not allow the article in its shops.
    /// </exception>
    public class ProduceInventoryItemHandler : ICommandHandler<ProduceInventoryItem>
    {
        private readonly IEntityRepository _repository;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderService _orderService;
        private readonly IEventTransmitter _eventTransmitter;

        public ProduceInventoryItemHandler(IEntityRepository repository,
            IInventoryService inventoryService, IOrderService orderService,
            IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _inventoryService = inventoryService;
            _orderService = orderService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ProduceInventoryItem command)
        {
            var shop = await _repository.GetAsync<Shop>(command.ShopId);
            var order = await _repository.GetAsync<Order>(command.OrderId);

            var produced = await _inventoryService.Produce(shop, command.ArticleId, command.Amount, order);
            _orderService.IncreaseProduced(produced, order);

            await _eventTransmitter.BroadcastAsync(new ShopInventoryChanged(shop, shop.Inventory, command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(
                new InventoryItemProduced(shop, produced, order, command.InitiatorId));
        }
    }
}
