using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Archives the specified shop category.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if category contains shops.
    /// </exception>
    public class ArchiveShopCategoryHandler : ICommandHandler<ArchiveShopCategory>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveShopCategoryHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveShopCategory command)
        {
            var shopCategory = await _repository.LoadAsync<ShopCategory>(command.Id);
            if (shopCategory.Shops.Any())
                throw new DomainException(ArchivingShopCategoryWithShops(shopCategory));

            await _repository.RemoveAsync(shopCategory);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
