using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Services;
using Epok.Domain.Shops.Repositories;
using Epok.Domain.Suppliers.Entities;
using Epok.Domain.Suppliers.Events;
using System.Threading.Tasks;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Receives materials from the supplier.
    /// </summary>
    public class ReceiveMaterialsHandler : ICommandHandler<ReceiveMaterials>
    {
        private readonly IEntityRepository _repository;
        private readonly IShopRepository _shopRepo;
        private readonly IInventoryService _inventoryService;
        private readonly IEventTransmitter _eventTransmitter;

        public ReceiveMaterialsHandler(IEntityRepository repository,
            IShopRepository shopRepo, IInventoryService inventoryService,
            IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _shopRepo = shopRepo;
            _inventoryService = inventoryService;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ReceiveMaterials command)
        {
            var materialRequest = await _repository.GetAsync<MaterialRequest>(command.MaterialRequestId);
            materialRequest.Status = MaterialRequestStatus.Fulfilled;
            var shop = await _shopRepo.GetEntryPoint();
            _inventoryService.IncreaseInventory(materialRequest.ItemsRequested, shop);

            await _eventTransmitter.BroadcastAsync(new MaterialsReceived(materialRequest, command.InitiatorId));
        }
    }
}
