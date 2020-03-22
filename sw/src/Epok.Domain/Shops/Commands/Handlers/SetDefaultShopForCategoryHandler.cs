using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Sets shop as default for the category.
    /// </summary>
    public class SetDefaultShopForCategoryHandler : ICommandHandler<SetDefaultShopForCategory>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public SetDefaultShopForCategoryHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(SetDefaultShopForCategory command)
        {
            var shopCategory = await _repository.GetAsync<ShopCategory>(command.ShopCategoryId);
            if (shopCategory.DefaultShop.Id == command.ShopId)
                return;
            shopCategory.DefaultShop.IsDefaultForCategory = false;
            shopCategory.Shops.Single(s => s.Id == command.ShopId).IsDefaultForCategory = true;

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Shop>(shopCategory.DefaultShop, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
