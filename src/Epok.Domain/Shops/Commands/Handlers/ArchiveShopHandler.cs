using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Repositories;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Archives the specified shop.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if shop has inventory.
    /// </exception>
    public class ArchiveShopHandler : ICommandHandler<ArchiveShop>
    {
        private readonly IShopRepository _shopRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveShopHandler(IShopRepository shopRepo, IEventTransmitter eventTransmitter)

        {
            _shopRepo = shopRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveShop command)
        {
            var shop = await _shopRepo.GetAsync(command.Id);
            if (shop.Inventory.Count != 0)
                throw new DomainException(ArchivingShopWithInventory(shop));

            await _shopRepo.ArchiveAsync(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Shop>(shop, Trigger.Removed, command.InitiatorId));
        }
    }
}
