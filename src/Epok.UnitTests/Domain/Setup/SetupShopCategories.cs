using Epok.Core.Utilities;
using Epok.Domain.Shops.Entities;
using FakeItEasy;
using System;

namespace Epok.UnitTests.Domain.Setup
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
            TimberComponentShopCategory.Shops = TimberComponentShop.Collect().ToHashSet();
            TimberComponentShopCategory.Articles.Add(Material1Timber);
            TimberComponentShopCategory.Articles.Add(Material2Foil);
            TimberComponentShopCategory.Articles.Add(Component1Vertical);
            TimberComponentShopCategory.Articles.Add(Component2Horizontal);

            MdfComponentShopCategory.Shops = MdfComponentShop.Collect().ToHashSet();
            MdfComponentShopCategory.Articles.Add(Material3Mdf);
            MdfComponentShopCategory.Articles.Add(Material2Foil);
            MdfComponentShopCategory.Articles.Add(Component3MdfFiller);

            GlassComponentShopCategory.Articles.Add(Material4TintedGlass);
            GlassComponentShopCategory.Articles.Add(Component4GlassFiller);
            GlassComponentShopCategory.Shops = GlassComponentShop.Collect().ToHashSet();

            ProductAssemblyShopCategory.Articles.Add(Component1Vertical);
            ProductAssemblyShopCategory.Articles.Add(Component2Horizontal);
            ProductAssemblyShopCategory.Articles.Add(Component3MdfFiller);
            ProductAssemblyShopCategory.Articles.Add(Component4GlassFiller);
            ProductAssemblyShopCategory.Articles.Add(Product1InteriorDoor);
            ProductAssemblyShopCategory.Articles.Add(Product2InteriorDoor);
            ProductAssemblyShopCategory.Articles.Add(Component1VerticalSpoiled);
            ProductAssemblyShopCategory.Articles.Add(Product1InteriorDoorSpoiled);
            ProductAssemblyShopCategory.Shops = ProductAssemblyShop.Collect().ToHashSet();

            MaterialStockpileShopCategory.Articles.Add(Material1Timber);
            MaterialStockpileShopCategory.Articles.Add(Material2Foil);
            MaterialStockpileShopCategory.Articles.Add(Material3Mdf);
            MaterialStockpileShopCategory.Articles.Add(Material4TintedGlass);
            MaterialStockpileShopCategory.Shops = MaterialStockpileShop.Collect().ToHashSet();

            ProductStockpileShopCategory.Articles.Add(Product1InteriorDoor);
            ProductStockpileShopCategory.Articles.Add(Product2InteriorDoor);
            ProductStockpileShopCategory.Shops = ProductStockpileShop.Collect().ToHashSet();

            ShopCategoryToArchive.Shops = ShopToRemove.Collect().ToHashSet();
            ShopCategoryToArchive.Articles.Add(ArticleToArchive);
        }

        private void StubShopCategoriesRepositories()
        {
            A.CallTo(() => ShopCategoryRepo.LoadAsync(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(MdfComponentShopCategory.Id)).Returns(MdfComponentShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(MdfComponentShopCategory.Id)).Returns(MdfComponentShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);
            A.CallTo(() => ShopCategoryRepo.GetAsync(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);

            A.CallTo(() => ShopCategoryRepo.LoadAsync(ShopCategoryToArchive.Id)).Returns(ShopCategoryToArchive);
            A.CallTo(() => ShopCategoryRepo.GetAsync(ShopCategoryToArchive.Id)).Returns(ShopCategoryToArchive);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(TimberComponentShopCategory.Id))
                .Returns(TimberComponentShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(MdfComponentShopCategory.Id))
                .Returns(MdfComponentShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(MdfComponentShopCategory.Id))
                .Returns(MdfComponentShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(GlassComponentShopCategory.Id))
                .Returns(GlassComponentShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(ProductAssemblyShopCategory.Id))
                .Returns(ProductAssemblyShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(MaterialStockpileShopCategory.Id))
                .Returns(MaterialStockpileShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(ProductStockpileShopCategory.Id))
                .Returns(ProductStockpileShopCategory);

            A.CallTo(() => ReadOnlyRepo.LoadAsync<ShopCategory>(ShopCategoryToArchive.Id))
                .Returns(ShopCategoryToArchive);
            A.CallTo(() => ReadOnlyRepo.GetAsync<ShopCategory>(ShopCategoryToArchive.Id))
                .Returns(ShopCategoryToArchive);
        }
    }
}
