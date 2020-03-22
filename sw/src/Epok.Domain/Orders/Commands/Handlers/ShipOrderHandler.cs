using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Events;
using Epok.Domain.Orders.Services;
using System.Threading.Tasks;
using static Epok.Domain.Orders.ExceptionCauses;

namespace Epok.Domain.Orders.Commands.Handlers
{
    /// <summary>
    /// Ships order to customer.
    /// </summary>
    /// <exception cref="OrderIsNotReadyForShipment">
    /// Thrown if order is not ready for the shipment
    /// i.e. not all items are produces or order is
    /// not at the exit point.
    /// </exception>
    public class ShipOrderHandler : ICommandHandler<ShipOrder>
    {
        private readonly IEntityRepository _repository;
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public ShipOrderHandler(IEntityRepository repository, IOrderService orderService,
            IInventoryService inventoryService, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _orderService = orderService;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ShipOrder command)
        {
            var order = await _repository.GetAsync<Order>(command.Id);

            Guard.Against.Null(order, nameof(order));

            if (order.Status != OrderStatus.ReadyForShipment)
                throw new DomainException(OrderIsNotReadyForShipment(order));

            _orderService.ShipOrder(order);
            _inventoryService.DecreaseInventory(order.ItemsProduced, order.Shop);

            await _eventTransmitter.BroadcastAsync(new OrderShipped(order, command.InitiatorId));
        }
    }
}
