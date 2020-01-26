using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Commands.Handlers;
using Epok.Domain.Inventory.Entities;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class InventoryArticleTests : SetupBase
    {
        [Test]
        public async Task RegisterArticle_Success()
        {
            //arrange
            const decimal inputM1 = 3;
            const decimal inputM2 = 5;
            const decimal inputM4 = 4;

            var command = new RegisterArticle()
            {
                Name = "New article",
                Code = "N001",
                UomId = PieceUom.Id,
                ArticleType = ArticleType.Product,
                ProductionShopCategoryId = ProductAssemblyShopCategory.Id,
                TimeToProduce = TimeSpan.FromHours(1),
                BomInput = new HashSet<(Guid, decimal)>
                {
                    (Material1Timber.Id, inputM1),
                    (Material2Foil.Id, inputM2),
                    (Material4TintedGlass.Id, inputM4)
                },
                BomOutput = 1,
                InitiatorId = GlobalAdmin.Id
            };

            var handler = new RegisterArticleHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.AddAsync)), Is.EqualTo(1));
            var article = GetRecordedEntities<Article>(EntityRepository, nameof(EntityRepository.AddAsync)).Single();
            Assert.That(article.Name, Is.EqualTo(command.Name));
            Assert.That(article.Code, Is.EqualTo(command.Code));
            Assert.That(article.UoM, Is.EqualTo(PieceUom));
            Assert.That(article.ArticleType, Is.EqualTo(command.ArticleType));
            Assert.That(article.ProductionShopCategory, Is.EqualTo(ProductAssemblyShopCategory));
            Assert.That(article.TimeToProduce, Is.EqualTo(command.TimeToProduce));

            var events = GetRecordedEvents<DomainEvent<Article>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(article));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public async Task ArchiveArticle_Success()
        {
            //arrange
            var command = new ArchiveArticle
            {
                Id = ArticleToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveArticleHandler(EntityRepository,InventoryRepo, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(InventoryRepo, nameof(InventoryRepo.ArchiveAsync)), Is.EqualTo(1));
            var events = GetRecordedEvents<DomainEvent<Article>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(ArticleToArchive));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public void ArchiveArticle_FailFor_ArticleInStock()
        {
            //arrange
            var command = new ArchiveArticle {Id = Product1InteriorDoor.Id};
            var handler = new ArchiveArticleHandler(EntityRepository, InventoryRepo, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message,
                Is.EqualTo(ArchivingArticleStillInStock(Product1InteriorDoor, AmountInStock(Product1InteriorDoor))));
            Assert.That(GetRecordedEvents<DomainEvent<Article>>(), Is.Empty);
        }

        [Test]
        public void ArchiveArticle_FailFor_ArticleInActiveOrders()
        {
            //arrange
            var command = new ArchiveArticle {Id = Product2InteriorDoor.Id};
            var handler = new ArchiveArticleHandler(EntityRepository, InventoryRepo, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingArticleStillInOrders(Product2InteriorDoor, 1)));
            Assert.That(GetRecordedEvents<DomainEvent<Article>>(), Is.Empty);
        }
    }
}
