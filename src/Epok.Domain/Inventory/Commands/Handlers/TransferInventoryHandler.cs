using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Events;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Events;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Transfers inventory between shops.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if lacking inventory to transfer
    /// or the target shop does not allow the article
    /// to be transferred.
    /// </exception>
    public class TransferInventoryHandler : ICommandHandler<TransferInventory>
    {
        private readonly IEntityRepository _repository;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public TransferInventoryHandler(IInventoryService inventoryService,
            IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(TransferInventory command)
        {
            var article = await _repository.GetAsync<Article>(command.ArticleId);
            var source = await _repository.GetAsync<Shop>(command.SourceShopId);
            var target = await _repository.GetAsync<Shop>(command.TargetShopId);

            var transferred = _inventoryService.Transfer(source, target, new InventoryItem(article, command.Amount));

            await _eventTransmitter.BroadcastAsync(new ShopInventoryChanged(source, source.Inventory,
                command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new ShopInventoryChanged(target, target.Inventory,
                command.InitiatorId));
            await _eventTransmitter.BroadcastAsync(new InventoryTransferred(source, target, transferred,
                command.InitiatorId));
        }
    }
}
