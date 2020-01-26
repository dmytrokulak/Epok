using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Shops.Commands.Handlers
{
    /// <summary>
    /// Creates a new shop category.
    /// </summary>
    public class CreateShopCategoryHandler : ICommandHandler<CreateShopCategory>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public CreateShopCategoryHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(CreateShopCategory command)
        {
            var shopCategory = new ShopCategory(command.Id, command.Name)
            {
                ShopType = command.ShopType
            };
            var articles = await _repository.GetSomeAsync<Article>(command.Articles);
            foreach (var article in articles)
                shopCategory.Articles.Add(article);

            await _repository.AddAsync(shopCategory);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<ShopCategory>(shopCategory, Trigger.Added,
                command.InitiatorId));
        }
    }
}
