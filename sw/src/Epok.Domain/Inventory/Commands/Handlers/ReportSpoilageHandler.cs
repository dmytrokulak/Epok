using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using System;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Creates a report on the inventory items
    /// spoiled due to production defects.
    /// </summary>
    public class ReportSpoilageHandler : ICommandHandler<ReportSpoilage>
    {
        private readonly IEntityRepository _repository;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public ReportSpoilageHandler(IEntityRepository repository, IInventoryService inventoryService,
            IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ReportSpoilage command)
        {
            var shop = await _repository.GetAsync<Shop>(command.ShopId);
            var order = await _repository.GetAsync<Order>(command.OrderId);

            var spoilageItem =
                await _inventoryService.ReportSpoilage(command.ArticleId, command.Amount, command.Fixable, order, shop);

            await _repository.AddAsync(new SpoilageReport(Guid.NewGuid(), $"{spoilageItem.Article} spoiled")
            {
                Item = spoilageItem,
                Reason = command.Reason
            });

            await _eventTransmitter.BroadcastAsync(new ShopInventoryChanged(shop, shop.Inventory, command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new SpoilageReported(shop, spoilageItem, command.InitiatorId));
        }
    }
}
