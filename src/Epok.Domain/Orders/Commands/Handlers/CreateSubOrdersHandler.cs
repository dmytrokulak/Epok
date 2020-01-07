using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Repositories;
using Epok.Domain.Orders.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Domain.Orders.Commands.Handlers
{
    /// <summary>
    /// Creates internal sub orders in shops.
    /// </summary>
    public class CreateSubOrdersHandler : ICommandHandler<CreateSubOrders>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderService _orderService;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateSubOrdersHandler(IOrderRepository orderRepo, IInventoryRepository inventoryRepo,
            IOrderService orderService, IEventTransmitter eventTransmitter)
        {
            _orderRepo = orderRepo;
            _orderService = orderService;
            _inventoryRepo = inventoryRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateSubOrders command)
        {
            var order = await _orderRepo.GetAsync(command.OrderId);
            var inventory = _orderService.CalculateInventoryInput(order);

            foreach (var item in inventory)
                item.Amount -= await _inventoryRepo.FindSpareInventoryAsync(item.Article);

            var subOrders = new List<Order>();

            foreach (var item in order.ItemsOrdered)
                await CreateSubOrder(item, order);

            async Task CreateSubOrder(InventoryItem item, Order parent)
            {
                var input = inventory.Of(item.Article);
                if (!(input.Amount > 0) || input.Article.ProductionShopCategory == null)
                    return;

                var subOrder = _orderService.CreateSubOrder(input, parent);
                subOrders.Add(subOrder);
                if (item.Article.PrimaryBillOfMaterial == null)
                    return;
                foreach (var subItem in item.Article.PrimaryBillOfMaterial.Input)
                    await CreateSubOrder(subItem, subOrder);

                await _orderRepo.AddAsync(subOrder);
            }

            foreach (var subOrder in subOrders)
                await _eventTransmitter.BroadcastAsync(new DomainEvent<Order>(subOrder, Trigger.Added,
                    command.InitiatorId));
        }
    }
}
