using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.UnitTests.Domain.ServiceTests
{
    [TestFixture]
    public class InventoryServiceTests : SetupBase
    {
        #region Increase Inventory Tests

        [Test]
        public void IncreaseInventory_FailFor_ArticleNotAllowed()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>();
            shop.ShopCategory = shop.ShopCategory.DeepCopy();
            shop.ShopCategory.Articles = new HashSet<Article>();
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //assert () => act
            Assert.Throws<DomainException>(() => InventoryService.IncreaseInventory(item, shop));
        }

        [Test]
        public void IncreaseInventory_ArticleWasNotInStockBefore_Success()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>();
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //act
            InventoryService.IncreaseInventory(item, shop);

            //assert
            var actual = shop.Inventory.Of(Product1InteriorDoor).Amount;

            Assert.That(actual, Is.EqualTo(item.Amount));
        }

        [Test]
        public void IncreaseInventory_ArticleHasBeenInStock_Success()
        {
            const decimal initial = 3;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initial)
            };
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //act
            InventoryService.IncreaseInventory(item, shop);

            //assert
            var actual = shop.Inventory.Of(Product1InteriorDoor).Amount;

            Assert.That(actual, Is.EqualTo(initial + item.Amount));
        }

        [Test]
        public void IncreaseInventory_MultipleArticles_Success()
        {
            const decimal initialP1 = 3;
            const decimal initialP2 = 0;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var items = new[]
            {
                new InventoryItem(Product1InteriorDoor, 5),
                new InventoryItem(Product2InteriorDoor, 6),
            };
            //act
            InventoryService.IncreaseInventory(items, shop);

            //assert
            var actualP1 = shop.Inventory.Of(Product1InteriorDoor).Amount;
            var actualP2 = shop.Inventory.Of(Product2InteriorDoor).Amount;

            Assert.That(actualP1, Is.EqualTo(initialP1 + items[0].Amount));
            Assert.That(actualP2, Is.EqualTo(initialP2 + items[1].Amount));
        }

        #endregion

        #region Decrease Inventory Tests

        [Test]
        public void DecreaseInventory_FailFor_ArticleNotInStock()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>();
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //assert () => act
            var ex = Assert.Throws<DomainException>(() => InventoryService.DecreaseInventory(item, shop));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotInStock(item.Article, shop)));
        }

        [Test]
        public void DecreaseInventory_FailFor_InSufficientInventory()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, 3)
            };
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //assert () => act
            var ex = Assert.Throws<DomainException>(() => InventoryService.DecreaseInventory(item, shop));
            Assert.That(ex.Message,
                Is.EqualTo(InsufficientInventory(item.Article, item.Amount, shop.Inventory.First().Amount)));
        }

        [Test]
        public void DecreaseInventory_SingleArticle_Success()
        {
            const decimal initial = 15;
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initial)
            };
            var item = new InventoryItem(Product1InteriorDoor, 5);

            //act
            InventoryService.DecreaseInventory(item, shop);

            //assert
            var actual = shop.Inventory.Of(Product1InteriorDoor).Amount;

            Assert.That(actual, Is.EqualTo(initial - item.Amount));
        }

        [Test]
        public void DecreaseInventory_MultipleArticles_Success()
        {
            const decimal initialP1 = 30;
            const decimal initialP2 = 10;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initialP1),
                new InventoryItem(Product2InteriorDoor, initialP2),
            };
            var items = new[]
            {
                new InventoryItem(Product1InteriorDoor, 5),
                new InventoryItem(Product2InteriorDoor, 6),
            };
            //act
            InventoryService.DecreaseInventory(items, shop);

            //assert
            var actualP1 = shop.Inventory.Of(Product1InteriorDoor).Amount;
            var actualP2 = shop.Inventory.Of(Product2InteriorDoor).Amount;

            Assert.That(actualP1, Is.EqualTo(initialP1 - items[0].Amount));
            Assert.That(actualP2, Is.EqualTo(initialP2 - items[1].Amount));
        }

        #endregion

        #region Produce Inventory Tests

        [Test]
        public void ProduceInventory_FailFor_ArticleNotAllowedInShop()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.ShopCategory = ProductAssemblyShopCategory.DeepCopy();
            shop.ShopCategory.Articles.Remove(Product2InteriorDoor);
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, 5),
                new InventoryItem(Component2Horizontal, 8),
                new InventoryItem(Component3MdfFiller, 10),
                new InventoryItem(Product1InteriorDoor, 1),
            };
            var item = new InventoryItem(Product2InteriorDoor, 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Produce(shop, item, Product1Order));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotAllowedInShop(item.Article, shop)));
        }

        [Test]
        public void ProduceInventory_FailFor_ArticleNotInOrder()
        {
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, 5),
                new InventoryItem(Component2Horizontal, 8),
                new InventoryItem(Component3MdfFiller, 10),
                new InventoryItem(Product1InteriorDoor, 1),
            };
            var item = new InventoryItem(Product2InteriorDoor, 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Produce(shop, item, Product1Order));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotInOrder(Product1Order, item.Article)));
        }

        [Test]
        public void ProduceInventory_FailFor_ProductionRequestExceedsRequirements()
        {
            decimal initialP1 = 1;
            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, 5),
                new InventoryItem(Component2Horizontal, 8),
                new InventoryItem(Component3MdfFiller, 10),
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var yetRequired = Product1Order.ItemsOrdered.Single(i => i.Article == Product1InteriorDoor).Amount -
                              Product1Order.ItemsProduced.Single(i => i.Article == Product1InteriorDoor).Amount;
            var item = new InventoryItem(Product1InteriorDoor, yetRequired * 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Produce(shop, item, Product1Order));
            Assert.That(ex.Message,
                Is.EqualTo(ProductionRequestExceedsRequirements(Product1Order, yetRequired, item.Article,
                    item.Amount)));
        }

        [Test]
        public void ProduceInventory_FailFor_ArticleNotInStock()
        {
            decimal initialC1 = 5;
            decimal initialC2 = 8;
            // decimal initialC3 = 10;
            decimal initialP1 = 1;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, initialC1),
                new InventoryItem(Component2Horizontal, initialC2),
                //  new InventoryItem(Component3MdfFiller, initialC3),
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var item = new InventoryItem(Product1InteriorDoor, 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Produce(shop, item, Product1Order));
            Assert.That(ex.Message,
                Is.EqualTo(ArticleNotInStock(Component3MdfFiller, shop)));
        }

        [Test]
        public void ProduceInventory_FailFor_InsufficientInventory()
        {
            decimal initialC1 = 5;
            decimal initialC2 = 8;
            decimal initialC3 = 1;
            decimal initialP1 = 1;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, initialC1),
                new InventoryItem(Component2Horizontal, initialC2),
                new InventoryItem(Component3MdfFiller, initialC3),
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var item = new InventoryItem(Product1InteriorDoor, 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Produce(shop, item, Product1Order));

            var input = Product1InteriorDoor.PrimaryBillOfMaterial.Input
                            .Of(Component3MdfFiller).Amount * item.Amount;
            Assert.That(ex.Message,
                Is.EqualTo(InsufficientInventory(Component3MdfFiller, input, initialC3)));
        }

        [Test]
        public void ProduceInventory_Success()
        {
            decimal initialC1 = 5;
            decimal initialC2 = 8;
            decimal initialC3 = 10;
            decimal initialP1 = 1;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, initialC1),
                new InventoryItem(Component2Horizontal, initialC2),
                new InventoryItem(Component3MdfFiller, initialC3),
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var item = new InventoryItem(Product1InteriorDoor, 2);
            var order = Product1Order.DeepCopy();

            //act
            InventoryService.Produce(shop, item, order);

            //assert
            var actualP1 = shop.Inventory.Of(Product1InteriorDoor).Amount;
            Assert.That(actualP1, Is.EqualTo(initialP1 + item.Amount));
            AssertInput(initialC1, Component1Vertical);
            AssertInput(initialC2, Component2Horizontal);
            AssertInput(initialC3, Component3MdfFiller);

            void AssertInput(decimal initial, Article article)
            {
                var actual = shop.Inventory.Of(article).Amount;
                var input = Product1InteriorDoor.PrimaryBillOfMaterial.Input.Of(article).Amount;
                Assert.That(actual, Is.EqualTo(initial - (input * item.Amount)));
            }
        }

        #endregion

        #region Transfer Inventory Tests

        [Test]
        public void TransferInventory_FailFor_ArticleNotAllowedInShop()
        {
            //arrange
            var source = MaterialStockpileShop.DeepCopy();
            var target = ProductStockpileShop.DeepCopy();
            var item = new InventoryItem(Material1Timber, 1);

            //assert () => act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Transfer(source, target, item));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotAllowedInShop(item.Article, target)));
        }

        [Test]
        public void TransferInventory_FailFor_ArticleNotInStock()
        {
            //arrange
            var source = ProductAssemblyShop.DeepCopy();
            source.Inventory = new HashSet<InventoryItem>();
            var target = ProductStockpileShop.DeepCopy();
            var item = new InventoryItem(Product1InteriorDoor, 2);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Transfer(source, target, item));
            Assert.That(ex.Message, Is.EqualTo(ArticleNotInStock(Product1InteriorDoor, source)));
        }

        [Test]
        public void TransferInventory_FailFor_InsufficientInventory()
        {
            decimal initialP1 = 2;
            //arrange
            var source = ProductAssemblyShop.DeepCopy();
            source.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initialP1)
            };
            var target = ProductStockpileShop.DeepCopy();
            var item = new InventoryItem(Product1InteriorDoor, 10);

            //assert ()=> act
            var ex = Assert.Throws<DomainException>(() => InventoryService.Transfer(source, target, item));
            Assert.That(ex.Message, Is.EqualTo(InsufficientInventory(Product1InteriorDoor, item.Amount, initialP1)));
        }

        [Test]
        public void TransferInventory_Success()
        {
            decimal initialSource = 10;
            decimal initialTarget = 2;
            //arrange
            var source = ProductAssemblyShop.DeepCopy();
            source.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initialSource)
            };
            var target = ProductStockpileShop.DeepCopy();
            target.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Product1InteriorDoor, initialTarget)
            };
            var item = new InventoryItem(Product1InteriorDoor, 2);

            //act 
            InventoryService.Transfer(source, target, item);

            //assert
            var actualSource = source.Inventory.Of(Product1InteriorDoor).Amount;
            Assert.That(actualSource, Is.EqualTo(initialSource - item.Amount));

            var actualTarget = target.Inventory.Of(Product1InteriorDoor).Amount;
            Assert.That(actualTarget, Is.EqualTo(initialTarget + item.Amount));
        }

        #endregion

        #region Spoilage tests

        [Test]
        public void ReportSpoilage_Product_Success()
        {
            decimal initialC1 = 5;
            decimal initialC2 = 8;
            decimal initialC3 = 10;
            decimal initialP1 = 1;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, initialC1),
                new InventoryItem(Component2Horizontal, initialC2),
                new InventoryItem(Component3MdfFiller, initialC3),
                new InventoryItem(Product1InteriorDoor, initialP1),
            };
            var item = new InventoryItem(Product1InteriorDoorSpoiled, 2);
            var order = Product1Order.DeepCopy();

            //act
            InventoryService.ReportSpoilage(item, order, shop);

            //assert
            var actualP1 = shop.Inventory.Of(Product1InteriorDoor).Amount;
            Assert.That(actualP1, Is.EqualTo(initialP1));

            var actualS1 = shop.Inventory.Of(Product1InteriorDoorSpoiled).Amount;
            Assert.That(actualS1, Is.EqualTo(0 + item.Amount));

            AssertInput(initialC1, Component1Vertical);
            AssertInput(initialC2, Component2Horizontal);
            AssertInput(initialC3, Component3MdfFiller);

            void AssertInput(decimal initial, Article article)
            {
                var actual = shop.Inventory.Of(article).Amount;
                var input = Product1InteriorDoor.PrimaryBillOfMaterial.Input.Of(article).Amount;
                Assert.That(actual, Is.EqualTo(initial - (input * item.Amount)));
            }
        }

        [Test]
        public void ReportSpoilage_Material_Success()
        {
            decimal initialC1 = 5;

            //arrange
            var shop = ProductAssemblyShop.DeepCopy();
            shop.Inventory = new HashSet<InventoryItem>()
            {
                new InventoryItem(Component1Vertical, initialC1),
            };
            var item = new InventoryItem(Component1VerticalSpoiled, 2);
            var order = Product1Order.DeepCopy();

            //act
            InventoryService.ReportSpoilage(item, order, shop);

            //assert
            var actualS1 = shop.Inventory.Of(Component1VerticalSpoiled).Amount;
            Assert.That(actualS1, Is.EqualTo(item.Amount));

            var actualC1 = shop.Inventory.Of(Component1Vertical).Amount;
            Assert.That(actualC1, Is.EqualTo(initialC1 - item.Amount));
        }

        #endregion

        #region Repository Based Tests

        [Ignore("ToDo")]
        [Test]
        public async Task CalculateTimeOfCompletion_Success()
        {
            //arrange
            var now = DateTimeOffset.Now;
            var item = new InventoryItem(Product1InteriorDoor, 20);
            var spare = await InventoryRepo.FindSpareInventoryAsync(Product1InteriorDoor);
            var expected = TimeProvider.Add(now, new InventoryItem(item.Article, item.Amount - spare).TimeToProduce);

            //act
            var eta = await InventoryService.CalculateTimeOfCompletion(item.Collect());

            //assert
            Assert.That(eta, Is.AtLeast(expected));
        }

        [Ignore("ToDo")]
        [Test]
        public async Task CalculateAllocatableAmount_Success()
        {
            var amount = await InventoryService.CalculateAllocatableAmount(Product1InteriorDoor);
            Assert.That(amount, Is.EqualTo(100));
        }

        #endregion
    }
}
