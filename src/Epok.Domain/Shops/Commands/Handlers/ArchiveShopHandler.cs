using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;
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
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveShopHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveShop command)
        {
            var shop = await _repository.GetAsync<Shop>(command.Id);
            if (shop.Inventory.Count != 0)
                throw new DomainException(ArchivingShopWithInventory(shop));

            await _repository.RemoveAsync(shop);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Shop>(shop, Trigger.Removed, command.InitiatorId));
        }
    }
}
