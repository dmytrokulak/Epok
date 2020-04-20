using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epok.Domain.Tests.Setup
{
    public abstract partial class SetupBase
    {
        protected Article Material1Timber = new Article(Guid.NewGuid(), "Timber");
        protected Article Material2Foil = new Article(Guid.NewGuid(), "Pvc Foil");
        protected Article Material3Mdf = new Article(Guid.NewGuid(), "MDF");
        protected Article Material4TintedGlass = new Article(Guid.NewGuid(), "Tinted Glass");

        protected Article Component1Vertical = new Article(Guid.NewGuid(), "Vertical component");
        protected Article Component2Horizontal = new Article(Guid.NewGuid(), "Horizontal component");
        protected Article Component3MdfFiller = new Article(Guid.NewGuid(), "Mdf Filler component");
        protected Article Component4GlassFiller = new Article(Guid.NewGuid(), "Glass filler component");

        protected Article Product1InteriorDoor = new Article(Guid.NewGuid(), "Finished interior door");
        protected Article Product2InteriorDoor = new Article(Guid.NewGuid(), "Finished interior door with glass.");
        protected Article ArticleToArchive = new Article(Guid.NewGuid(), "Article to remove");

        protected SpoiledArticle Product1InteriorDoorSpoiled =
            new SpoiledArticle(Guid.NewGuid(), "Finished interior door spoiled");

        protected SpoiledArticle Component1VerticalSpoiled =
            new SpoiledArticle(Guid.NewGuid(), "Vertical component spoiled");

        protected Uom PieceUom = new Uom(Guid.Parse("00000000-0000-0000-0008-010510199101"), "Piece", UomType.Piece);

        protected Uom CubicMeterUom =
            new Uom(Guid.Parse("71177098-1050-9903-2077-101116101114"), "Cubic Meter", UomType.Volume);

        protected Uom SquareMeterUom =
            new Uom(Guid.Parse("13117097-1141-0103-2077-101116101114"), "Square Meter", UomType.Area);

        private void InitInventory()
        {
            Material1Timber.ArticleType = ArticleType.Material;
            Material1Timber.Code = "M001";
            Material1Timber.UoM = CubicMeterUom;

            Material2Foil.ArticleType = ArticleType.Material;
            Material2Foil.Code = "M002";
            Material2Foil.UoM = SquareMeterUom;

            Material3Mdf.ArticleType = ArticleType.Material;
            Material3Mdf.Code = "M003";
            Material3Mdf.UoM = SquareMeterUom;

            Material4TintedGlass.ArticleType = ArticleType.Material;
            Material4TintedGlass.Code = "M004";
            Material4TintedGlass.UoM = SquareMeterUom;

            Component1Vertical.ArticleType = ArticleType.ManufacturedComponent;
            Component1Vertical.Code = "C001";
            Component1Vertical.UoM = PieceUom;
            Component1Vertical.TimeToProduce = TimeSpan.FromMinutes(10);
            Component1Vertical.BillsOfMaterial = new BillOfMaterial(Guid.NewGuid(), "Vertical component BOM")
            {
                Input = new HashSet<InventoryItem>
                {
                    new InventoryItem(Material1Timber, 0.1M),
                    new InventoryItem(Material2Foil, 0.2M),
                },
                Output = 1,
                Primary = true
            }.CollectToHashSet();
            Component1Vertical.ProductionShopCategory = TimberComponentShopCategory;

            Component2Horizontal.ArticleType = ArticleType.ManufacturedComponent;
            Component2Horizontal.Code = "C002";
            Component2Horizontal.UoM = PieceUom;
            Component2Horizontal.TimeToProduce = TimeSpan.FromMinutes(10);
            Component2Horizontal.BillsOfMaterial = new HashSet<BillOfMaterial>()
            {
                new BillOfMaterial(Guid.NewGuid(), "Horizontal component BOM")
                {
                    Article = Component2Horizontal,
                    Input = new HashSet<InventoryItem>
                    {
                        new InventoryItem(Material1Timber, 0.05M),
                        new InventoryItem(Material2Foil, 0.1M),
                    },
                    Output = 1,
                    Primary = true
                },
                new BillOfMaterial(Guid.NewGuid(), "Horizontal component BOM from vertical spoilage.")
                {
                    Article = Component2Horizontal,
                    Input = new InventoryItem(Component1VerticalSpoiled, 1).CollectToHashSet(),
                    Output = 1,
                    Primary = false
                }
            };
            Component2Horizontal.ProductionShopCategory = TimberComponentShopCategory;

            Component3MdfFiller.ArticleType = ArticleType.ManufacturedComponent;
            Component3MdfFiller.Code = "C003";
            Component3MdfFiller.UoM = PieceUom;
            Component3MdfFiller.TimeToProduce = TimeSpan.FromMinutes(5);
            Component3MdfFiller.BillsOfMaterial = new BillOfMaterial(Guid.NewGuid(), "Mdf Filler component BOM")
            {
                Input = new HashSet<InventoryItem>
                {
                    new InventoryItem(Material3Mdf, 0.1M),
                    new InventoryItem(Material2Foil, 0.1M),
                },
                Output = 1,
                Primary = true
            }.CollectToHashSet();
            Component3MdfFiller.ProductionShopCategory = MdfComponentShopCategory;

            Component4GlassFiller.ArticleType = ArticleType.ManufacturedComponent;
            Component4GlassFiller.Code = "C004";
            Component4GlassFiller.UoM = PieceUom;
            Component4GlassFiller.TimeToProduce = TimeSpan.FromMinutes(5);
            Component4GlassFiller.BillsOfMaterial = new BillOfMaterial(Guid.NewGuid(), "Glass Filler component BOM")
            {
                Input = new HashSet<InventoryItem>
                {
                    new InventoryItem(Material4TintedGlass, 0.1M),
                },
                Output = 1,
                Primary = true
            }.CollectToHashSet();
            Component4GlassFiller.ProductionShopCategory = GlassComponentShopCategory;

            Product1InteriorDoor.ArticleType = ArticleType.Product;
            Product1InteriorDoor.Code = "P001";
            Product1InteriorDoor.UoM = PieceUom;
            Product1InteriorDoor.TimeToProduce = TimeSpan.FromMinutes(60);
            Product1InteriorDoor.BillsOfMaterial = new BillOfMaterial(Guid.NewGuid(), "Finished interior door BOM")
            {
                Article = Product1InteriorDoor,
                Input = new HashSet<InventoryItem>
                {
                    new InventoryItem(Component1Vertical, 2M),
                    new InventoryItem(Component2Horizontal, 2M),
                    new InventoryItem(Component3MdfFiller, 2M),
                },
                Output = 1,
                Primary = true
            }.CollectToHashSet();
            Product1InteriorDoor.ProductionShopCategory = ProductAssemblyShopCategory;

            Product2InteriorDoor.ArticleType = ArticleType.Product;
            Product2InteriorDoor.Code = "P002";
            Product2InteriorDoor.UoM = PieceUom;
            Product2InteriorDoor.TimeToProduce = TimeSpan.FromMinutes(60);
            Product2InteriorDoor.BillsOfMaterial =
                new BillOfMaterial(Guid.NewGuid(), "Finished interior door with glass BOM")
                {
                    Input = new HashSet<InventoryItem>
                    {
                        new InventoryItem(Component1Vertical, 2M),
                        new InventoryItem(Component2Horizontal, 2M),
                        new InventoryItem(Component3MdfFiller, 1M),
                        new InventoryItem(Component4GlassFiller, 1M),
                    },
                    Output = 1,
                    Primary = true
                }.CollectToHashSet();
            Product2InteriorDoor.ProductionShopCategory = ProductAssemblyShopCategory;

            Product1InteriorDoorSpoiled.Article = Product1InteriorDoor;
            Product1InteriorDoorSpoiled.Fixable = true;
            Product1InteriorDoorSpoiled.ProductionShopCategory = ProductAssemblyShopCategory;

            Component1VerticalSpoiled.Article = Component1Vertical;
            Component1VerticalSpoiled.Fixable = true;
            Component1VerticalSpoiled.ProductionShopCategory = TimberComponentShopCategory;
        }

        private void StubInventoryRepositories()
        {
            #region EntityRepository LoadAsync GetAsync
            A.CallTo(() => EntityRepository.LoadAsync<Article>(Product1InteriorDoor.Id)).Returns(Product1InteriorDoor);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Product1InteriorDoor.Id)).Returns(Product1InteriorDoor);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Product2InteriorDoor.Id)).Returns(Product2InteriorDoor);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Product2InteriorDoor.Id)).Returns(Product2InteriorDoor);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Material1Timber.Id)).Returns(Material1Timber);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Material1Timber.Id)).Returns(Material1Timber);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Material2Foil.Id)).Returns(Material2Foil);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Material2Foil.Id)).Returns(Material2Foil);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Material4TintedGlass.Id)).Returns(Material4TintedGlass);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Material4TintedGlass.Id)).Returns(Material4TintedGlass);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Material3Mdf.Id)).Returns(Material3Mdf);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Material3Mdf.Id)).Returns(Material3Mdf);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Component1Vertical.Id)).Returns(Component1Vertical);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Component1Vertical.Id)).Returns(Component1Vertical);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Component2Horizontal.Id)).Returns(Component2Horizontal);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Component2Horizontal.Id)).Returns(Component2Horizontal);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Component3MdfFiller.Id)).Returns(Component3MdfFiller);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Component3MdfFiller.Id)).Returns(Component3MdfFiller);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(Component4GlassFiller.Id)).Returns(Component4GlassFiller);
            A.CallTo(() => EntityRepository.GetAsync<Article>(Component4GlassFiller.Id)).Returns(Component4GlassFiller);

            A.CallTo(() => EntityRepository.LoadAsync<Article>(ArticleToArchive.Id)).Returns(ArticleToArchive);
            A.CallTo(() => EntityRepository.GetAsync<Article>(ArticleToArchive.Id)).Returns(ArticleToArchive);

            #endregion

            #region ArticleRepository LoadAsync GetAsync

            A.CallTo(() => ArticleRepo.LoadAsync(Product1InteriorDoor.Id)).Returns(Product1InteriorDoor);
            A.CallTo(() => ArticleRepo.GetAsync(Product1InteriorDoor.Id)).Returns(Product1InteriorDoor);

            A.CallTo(() => ArticleRepo.LoadAsync(Product2InteriorDoor.Id)).Returns(Product2InteriorDoor);
            A.CallTo(() => ArticleRepo.GetAsync(Product2InteriorDoor.Id)).Returns(Product2InteriorDoor);

            A.CallTo(() => ArticleRepo.LoadAsync(Material1Timber.Id)).Returns(Material1Timber);
            A.CallTo(() => ArticleRepo.GetAsync(Material1Timber.Id)).Returns(Material1Timber);

            A.CallTo(() => ArticleRepo.LoadAsync(Material2Foil.Id)).Returns(Material2Foil);
            A.CallTo(() => ArticleRepo.GetAsync(Material2Foil.Id)).Returns(Material2Foil);

            A.CallTo(() => ArticleRepo.LoadAsync(Material4TintedGlass.Id)).Returns(Material4TintedGlass);
            A.CallTo(() => ArticleRepo.GetAsync(Material4TintedGlass.Id)).Returns(Material4TintedGlass);

            A.CallTo(() => ArticleRepo.LoadAsync(Material3Mdf.Id)).Returns(Material3Mdf);
            A.CallTo(() => ArticleRepo.GetAsync(Material3Mdf.Id)).Returns(Material3Mdf);

            A.CallTo(() => ArticleRepo.LoadAsync(Component1Vertical.Id)).Returns(Component1Vertical);
            A.CallTo(() => ArticleRepo.GetAsync(Component1Vertical.Id)).Returns(Component1Vertical);

            A.CallTo(() => ArticleRepo.LoadAsync(Component2Horizontal.Id)).Returns(Component2Horizontal);
            A.CallTo(() => ArticleRepo.GetAsync(Component2Horizontal.Id)).Returns(Component2Horizontal);

            A.CallTo(() => ArticleRepo.LoadAsync(Component3MdfFiller.Id)).Returns(Component3MdfFiller);
            A.CallTo(() => ArticleRepo.GetAsync(Component3MdfFiller.Id)).Returns(Component3MdfFiller);

            A.CallTo(() => ArticleRepo.LoadAsync(Component4GlassFiller.Id)).Returns(Component4GlassFiller);
            A.CallTo(() => ArticleRepo.GetAsync(Component4GlassFiller.Id)).Returns(Component4GlassFiller);

            A.CallTo(() => ArticleRepo.LoadAsync(ArticleToArchive.Id)).Returns(ArticleToArchive);
            A.CallTo(() => ArticleRepo.GetAsync(ArticleToArchive.Id)).Returns(ArticleToArchive);

            #endregion

            var ids0 = Product1InteriorDoor.BillsOfMaterial.First().Input.Select(i => i.Article.Id);
            A.CallTo(() => EntityRepository.LoadSomeAsync(A<IEnumerable<Guid>>.That.IsSameSequenceAs(ids0), A<Expression<Func<Article,bool>>>.Ignored, A<int?>.Ignored, A<int?>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(Product1InteriorDoor.BillsOfMaterial.First().Input.Select(i => i.Article).ToList());

            var ids1 = new[] { Component1Vertical.Id, Component2Horizontal.Id, Component3MdfFiller.Id, Material2Foil.Id };
            A.CallTo(() => EntityRepository.LoadSomeAsync(A<IEnumerable<Guid>>.That.IsSameSequenceAs(ids1), A<Expression<Func<Article, bool>>>.Ignored, A<int?>.Ignored, A<int?>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new List<Article> { Component1Vertical, Component2Horizontal, Component3MdfFiller, Material2Foil });

            A.CallTo(() => ArticleRepo.FindSpoiledCounterpartAsync(Product1InteriorDoor, true))
                .Returns(Product1InteriorDoorSpoiled);
            A.CallTo(() => ArticleRepo.FindSpoiledCounterpartAsync(Component1Vertical, true))
                .Returns(Component1VerticalSpoiled);

            A.CallTo(() => EntityRepository.GetSomeAsync(A<IEnumerable<Guid>>.That.Contains(Product1InteriorDoor.Id), A<Expression<Func<Article, bool>>>.Ignored, A<int?>.Ignored, A<int?>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new[] { Product1InteriorDoor });

            A.CallTo(() => EntityRepository.GetSomeAsync(A<IEnumerable<Guid>>.That.Contains(Material2Foil.Id), A<Expression<Func<Article, bool>>>.Ignored, A<int?>.Ignored, A<int?>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new[] { Material2Foil });

            A.CallTo(() => EntityRepository.LoadAsync<Uom>(PieceUom.Id)).Returns(PieceUom);
            A.CallTo(() => EntityRepository.GetAsync<Uom>(PieceUom.Id)).Returns(PieceUom);

            A.CallTo(() => EntityRepository.LoadAsync<Uom>(CubicMeterUom.Id)).Returns(CubicMeterUom);
            A.CallTo(() => EntityRepository.GetAsync<Uom>(CubicMeterUom.Id)).Returns(CubicMeterUom);

            A.CallTo(() => EntityRepository.LoadAsync<BillOfMaterial>(Component2Horizontal.PrimaryBillOfMaterial.Id))
                .Returns(Component2Horizontal.PrimaryBillOfMaterial);
            A.CallTo(() => EntityRepository.GetAsync<BillOfMaterial>(Component2Horizontal.PrimaryBillOfMaterial.Id))
                .Returns(Component2Horizontal.PrimaryBillOfMaterial);

            A.CallTo(() => EntityRepository.LoadAsync<BillOfMaterial>(Product1InteriorDoor.PrimaryBillOfMaterial.Id))
                .Returns(Product1InteriorDoor.PrimaryBillOfMaterial);
            A.CallTo(() => EntityRepository.GetAsync<BillOfMaterial>(Product1InteriorDoor.PrimaryBillOfMaterial.Id))
                .Returns(Product1InteriorDoor.PrimaryBillOfMaterial);

            A.CallTo(() => EntityRepository.LoadAsync<BillOfMaterial>(Product2InteriorDoor.PrimaryBillOfMaterial.Id))
                .Returns(Product2InteriorDoor.PrimaryBillOfMaterial);
            A.CallTo(() => EntityRepository.GetAsync<BillOfMaterial>(Product2InteriorDoor.PrimaryBillOfMaterial.Id))
                .Returns(Product2InteriorDoor.PrimaryBillOfMaterial);

            var inStockP1 = Shops.SelectMany(s => s.Inventory).Where(i => i.Article == Product1InteriorDoor)
                .Sum(i => i.Amount);
            A.CallTo(() => InventoryRepo.FindTotalAmountInStockAsync(Product1InteriorDoor)).Returns(inStockP1);

            var inOrdersP1 = Orders.SelectMany(s => s.ItemsOrdered).Where(i => i.Article == Product1InteriorDoor)
                .Sum(i => i.Amount);
            A.CallTo(() => InventoryRepo.FindSpareInventoryAsync(Product1InteriorDoor)).Returns(inStockP1 - inOrdersP1);

            var inStockC1 = Shops.SelectMany(s => s.Inventory).Where(i => i.Article == Component1Vertical)
                .Sum(i => i.Amount);
            A.CallTo(() => InventoryRepo.FindTotalAmountInStockAsync(Component1Vertical)).Returns(inStockC1);
            A.CallTo(() => InventoryRepo.FindSpareInventoryAsync(Component1Vertical)).Returns(inStockC1);

            var inStockC2 = Shops.SelectMany(s => s.Inventory).Where(i => i.Article == Component2Horizontal)
                .Sum(i => i.Amount);
            A.CallTo(() => InventoryRepo.FindTotalAmountInStockAsync(Component2Horizontal)).Returns(inStockC2);
            A.CallTo(() => InventoryRepo.FindSpareInventoryAsync(Component2Horizontal)).Returns(inStockC2);

            var inStockC3 = Shops.SelectMany(s => s.Inventory).Where(i => i.Article == Component3MdfFiller)
                .Sum(i => i.Amount);
            A.CallTo(() => InventoryRepo.FindTotalAmountInStockAsync(Component3MdfFiller)).Returns(inStockC3);
            A.CallTo(() => InventoryRepo.FindSpareInventoryAsync(Component3MdfFiller)).Returns(inStockC3);

            A.CallTo(() => InventoryRepo.FindTotalAmountInOrdersAsync(Product2InteriorDoor)).Returns(1);

            A.CallTo(() => InventoryRepo.FindTotalAmountInOrdersAsync(Material1Timber))
                .Returns(AmountInOrders(Material1Timber));
            A.CallTo(() => InventoryRepo.FindTotalAmountInOrdersAsync(Material2Foil))
                .Returns(AmountInOrders(Material2Foil));
            A.CallTo(() => InventoryRepo.FindTotalAmountInOrdersAsync(Material3Mdf)).Returns(AmountInOrders(Material3Mdf));
            A.CallTo(() => InventoryRepo.FindTotalAmountInOrdersAsync(Material4TintedGlass))
                .Returns(AmountInOrders(Material4TintedGlass));
        }
    }
}
