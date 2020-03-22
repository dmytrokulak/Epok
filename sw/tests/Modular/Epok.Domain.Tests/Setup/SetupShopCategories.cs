using Epok.Core.Utilities;
using Epok.Domain.Shops.Entities;
using FakeItEasy;
using System;

namespace Epok.Domain.Tests.Setup
{
    public abstract partial class SetupBase
    {
        protected ShopCategory TimberComponentShopCategory =
            new ShopCategory(Guid.NewGuid(), "Timber component shop category");

        protected ShopCategory MdfComponentShopCategory =
            new ShopCategory(Guid.NewGuid(), "MDF component shop category");

        protected ShopCategory GlassComponentShopCategory =
            new ShopCategory(Guid.NewGuid(), "Glass component shop category");

        protected ShopCategory ProductAssemblyShopCategory = new ShopCategory(Guid.NewGuid(), "Doors shop category");

        protected ShopCategory MaterialStockpileShopCategory =
            new ShopCategory(Guid.NewGuid(), "Materials stockpile category");

        protected ShopCategory ProductStockpileShopCategory =
            new ShopCategory(Guid.NewGuid(), "Products stockpile category");

        protected ShopCategory ShopCategoryToArchive = new ShopCategory(Guid.NewGuid(), "Shop Category To Archive");

        private void InitShopCategories()
        {
            TimberComponentShopCategory.Shops = TimberComponentShop.CollectToHashSet();
            TimberComponentShopCategory.Articles.Add(Material1Timber);
            TimberComponentShopCategory.Articles.Add(Material2Foil);
            TimberComponentShopCategory.Articles.Add(Component1Vertical);
            TimberComponentShopCategory.Articles.Add(Component2Horizontal);

            MdfComponentShopCategory.Shops = MdfComponentShop.CollectToHashSet();
            MdfComponentShopCategory.Articles.Add(Material3Mdf);
            MdfComponentShopCategory.Articles.Add(Material2Foil);
            MdfComponentShopCategory.Articles.Add(Component3MdfFiller);

            GlassComponentShopCategory.Articles.Add(Material4TintedGlass);
            GlassComponentShopCategory.Articles.Add(Component4GlassFiller);
            GlassComponentShopCategory.Shops = GlassComponentShop.CollectToHashSet();

            ProductAssemblyShopCategory.Articles.Add(Component1Vertical);
            ProductAssemblyShopCategory.Articles.Add(Component2Horizontal);
            ProductAssemblyShopCategory.Articles.Add(Component3MdfFiller);
            ProductAssemblyShopCategory.Articles.Add(Component4GlassFiller);
            ProductAssemblyShopCategory.Articles.Add(Product1InteriorDoor);
            ProductAssemblyShopCategory.Articles.Add(Product2InteriorDoor);
            ProductAssemblyShopCategory.Articles.Add(Component1VerticalSpoiled);
            ProductAssemblyShopCategory.Articles.Add(Product1InteriorDoorSpoiled);
            ProductAssemblyShopCategory.Shops = ProductAssemblyShop.CollectToHashSet();

            MaterialStockpileShopCategory.Articles.Add(Material1Timber);
            MaterialStockpileShopCategory.Articles.Add(Material2Foil);
            MaterialStockpileShopCategory.Articles.Add(Material3Mdf);
            MaterialStockpileShopCategory.Articles.Add(Material4TintedGlass);
            MaterialStockpileShopCategory.Shops = MaterialStockpileShop.CollectToHashSet();

            ProductStockpileShopCategory.Articles.Add(Product1InteriorDoor);
            ProductStockpileShopCategory.Articles.Add(Product2InteriorDoor);
            ProductStockpileShopCategory.Shops = ProductStockpileShop.CollectToHashSet();

            ShopCategoryToArchive.Shops = ShopToRemove.CollectToHashSet();
            ShopCategoryToArchive.Articles.Add(ArticleToArchive);
        }

        private void StubShopCategoriesRepositories()
        {
            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(MdfComponentShopCategory.Id))
                .Returns(MdfComponentShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(MdfComponentShopCategory.Id))
                .Returns(MdfComponentShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);

            A.CallTo(() => EntityRepository.LoadAsync<ShopCategory>(ShopCategoryToArchive.Id))
                .Returns(ShopCategoryToArchive);
            A.CallTo(() => EntityRepository.GetAsync<ShopCategory>(ShopCategoryToArchive.Id))
                .Returns(ShopCategoryToArchive);
        }
    }
}
