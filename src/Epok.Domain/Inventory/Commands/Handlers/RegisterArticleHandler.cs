using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Persistence;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Registers new article in the system. 
    /// Creates a corresponding bill of material.
    /// Allows it in a shop category if production shop 
    /// category is specified.
    /// </summary>
    public class RegisterArticleHandler : ICommandHandler<RegisterArticle>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterArticleHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)

        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterArticle command)
        {

            ShopCategory shops = null;
            if (command.ProductionShopCategoryId != null)
                shops = await _repository.GetAsync<ShopCategory>(command.ProductionShopCategoryId.Value);

            Guard.Against.Null(shops, nameof(shops));

            var input = (await _repository.LoadSomeAsync<Article>(command.BomInput.Select(i => i.articleId)))
                .Select(a => new InventoryItem(a, command.BomInput.Single(i => i.articleId == a.Id).amount))
                .ToHashSet();

            var bom = new BillOfMaterial(Guid.NewGuid(), command.Name)
            {
                Input = input,
                Output = command.BomOutput,
                Primary = true
            };

            var article = new Article(command.Id, command.Name)
            {
                ArticleType = command.ArticleType,
                UoM = await _repository.LoadAsync<Uom>(command.UomId),
                Code = command.Code,
                BillsOfMaterial = bom.Collect().ToHashSet(),
                ProductionShopCategory = shops,
                TimeToProduce = command.TimeToProduce
            };

            // ReSharper disable once PossibleNullReferenceException : guarded against null above
            shops.Articles.Add(article);

            await _repository.AddAsync(article);
            await _eventTransmitter.BroadcastAsync(
                new DomainEvent<Article>(article, Trigger.Added, command.InitiatorId));
        }
    }
}
