using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Utilities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Persistence;

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
        private readonly IReadOnlyRepository _repo;
        private readonly IArticleRepository _articleRepo;
        private readonly IRepository<BillOfMaterial> _bomRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public RegisterArticleHandler(IReadOnlyRepository repo, IArticleRepository articleRepo,
            IRepository<BillOfMaterial> bomRepo, IEventTransmitter eventTransmitter)

        {
            _repo = repo;
            _articleRepo = articleRepo;
            _bomRepo = bomRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(RegisterArticle command)
        {
            var input = new HashSet<InventoryItem>();
            foreach (var (articleId, amount) in command.BomInput)
                input.Add(new InventoryItem(await _articleRepo.LoadAsync(articleId), amount));

            var bom = new BillOfMaterial(Guid.NewGuid(), command.Name)
            {
                Input = input,
                Output = command.BomOutput,
                Primary = true
            };
            await _bomRepo.AddAsync(bom);

            ShopCategory shops = null;
            if (command.ProductionShopCategoryId != null)
                shops = await _repo.GetAsync<ShopCategory>(command.ProductionShopCategoryId.Value);

            Guard.Against.Null(shops, nameof(shops));

            var article = new Article(command.Id, command.Name)
            {
                ArticleType = command.ArticleType,
                UoM = await _repo.LoadAsync<Uom>(command.UomId),
                Code = command.Code,
                BillsOfMaterial = bom.Collect().ToHashSet(),
                ProductionShopCategory = shops,
                TimeToProduce = command.TimeToProduce
            };

            shops.Articles.Add(article);

            await _articleRepo.AddAsync(article);
            await _eventTransmitter.BroadcastAsync(
                new DomainEvent<Article>(article, Trigger.Added, command.InitiatorId));
        }
    }
}
