using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Domain.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;
using static Epok.Domain.Orders.ExceptionCauses;

namespace Epok.Domain.Orders.Commands.Handlers
{
    /// <summary>
    /// Creates a new external order.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if materials in stock
    /// is not enough to produce the items ordered
    /// or estimated time to of production exceeds the
    /// shipment deadline.
    /// </exception>
    public class CreateOrderHandler : ICommandHandler<CreateOrder>
    {
        private readonly IReadOnlyRepository _repo;
        private readonly IOrderRepository _orderRepo;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateOrderHandler(IOrderRepository orderRepo, IReadOnlyRepository repo,
            IInventoryService inventoryService, IEventTransmitter eventTransmitter)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateOrder command)
        {
            var customer = await _repo.GetAsync<Customer>(command.CustomerId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //load items 
            var itemsOrdered = new HashSet<InventoryItem>();
            foreach (var (articleId, amount) in command.Items)
            {
                var article = await _repo.LoadAsync<Article>(articleId);
                itemsOrdered.Add(new InventoryItem(article, amount));
            }

            //check stocks availability
            foreach (var item in itemsOrdered)
            {
                var allocatable = await _inventoryService.CalculateAllocatableAmount(item.Article);
                if (item.Amount > allocatable)
                    throw new DomainException(InsufficientInventory(item.Article, item.Amount, allocatable));
            }

            //check time
            var eta = await _inventoryService.CalculateTimeOfCompletion(itemsOrdered);
            if (eta > command.ShipmentDeadline)
                throw new DomainException(InsufficientTime(command.Name, command.ShipmentDeadline, eta));

            //Create order
            var order = new Order(command.Id, command.Name)
            {
                Status = OrderStatus.New,
                ItemsOrdered = itemsOrdered,
                ItemsProduced = itemsOrdered.Select(i => InventoryItem.Empty(i.Article)).ToHashSet(),
                ShipmentDeadline = command.ShipmentDeadline,
                Customer = customer,
                Type = command.OrderType
            };

            //Save order and reference in db
            await _orderRepo.AddAsync(order);
            customer.Orders.Add(order);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Order>(order, Trigger.Added, command.InitiatorId));
        }
    }
}
