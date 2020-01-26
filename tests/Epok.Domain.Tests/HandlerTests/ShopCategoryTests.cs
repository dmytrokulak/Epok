using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Shops.Commands;
using Epok.Domain.Shops.Commands.Handlers;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Tests.Setup;
using NUnit.Framework;
using System.Threading.Tasks;
using static Epok.Domain.Shops.ExceptionCauses;

namespace Epok.Domain.Tests.HandlerTests
{
    [TestFixture]
    public class ShopCategoryTests : SetupBase
    {
        [Test]
        public async Task CreateShopCategory_Success()
        {
            //arrange
            var command = new CreateShopCategory()
            {
                Name = "New shop category",
                ShopType = Epok.Domain.Shops.ShopType.Workshop,
                Articles = new[] { Product1InteriorDoor.Id },
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new CreateShopCategoryHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities<ShopCategory>(EntityRepository, nameof(EntityRepository.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].Name, Is.EqualTo(command.Name));
            Assert.That(entities[0].ShopType, Is.EqualTo(command.ShopType));
            Assert.True(entities[0].Articles.Contains(Product1InteriorDoor));

            var events = GetRecordedEvents<DomainEvent<ShopCategory>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public async Task ArchiveShopCategory_Success()
        {
            //adhere
            var initialShops = ShopCategoryToArchive.Shops.DeepCopy();

            //arrange
            ShopCategoryToArchive.Shops.Remove(ShopToRemove);
            var command = new ArchiveShopCategory()
            {
                Id = ShopCategoryToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };

            var handler = new ArchiveShopCategoryHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var ids = GetRecordedIds(EntityRepository, nameof(EntityRepository.ArchiveAsync));
            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(command.Id));
            var events = GetRecordedEvents<DomainEvent<ShopCategory>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].Entity, Is.EqualTo(ShopCategoryToArchive));

            //annul
            ShopCategoryToArchive.Shops = initialShops;
        }

        [Test]
        public void ArchiveShopCategory_FailFor_ArchivingShopCategoryWithShops()
        {
            //arrange
            var command = new ArchiveShopCategory()
            {
                Id = ShopCategoryToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };

            var handler = new ArchiveShopCategoryHandler(EntityRepository, EventTransmitter);

            //assert ()=> act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingShopCategoryWithShops(ShopCategoryToArchive)));
        }

        [Test]
        public async Task AllowArticle_Success()
        {
            //adhere 
            var initialArticles = ShopCategoryToArchive.Articles.DeepCopy();

            //arrange
            var command = new AllowArticle()
            {
                ArticleId = Product1InteriorDoor.Id,
                ShopCategoryId = ShopCategoryToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new AllowArticleHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var events = GetRecordedEvents<DomainEvent<ShopCategory>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Changed));
            Assert.That(events[0].Entity, Is.EqualTo(ShopCategoryToArchive));

            Assert.False(initialArticles.Contains(Product1InteriorDoor));
            Assert.True(ShopCategoryToArchive.Articles.Contains(Product1InteriorDoor));

            //annul
            ShopCategoryToArchive.Articles = initialArticles;
        }

        [Test]
        public async Task DisallowArticle_Success()
        {
            //adhere 
            var initialArticles = ShopCategoryToArchive.Articles.DeepCopy();

            //arrange
            var command = new DisallowArticle()
            {
                ArticleId = ArticleToArchive.Id,
                ShopCategoryId = ShopCategoryToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new DisallowArticleHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var events = GetRecordedEvents<DomainEvent<ShopCategory>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Changed));
            Assert.That(events[0].Entity, Is.EqualTo(ShopCategoryToArchive));

            Assert.True(initialArticles.Contains(ArticleToArchive));
            Assert.False(ShopCategoryToArchive.Articles.Contains(ArticleToArchive));

            //annul
            ShopCategoryToArchive.Articles = initialArticles;
        }
    }
}
