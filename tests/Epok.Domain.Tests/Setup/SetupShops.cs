using Epok.Domain.Inventory.Entities;
using Epok.Domain.Shops.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Tests.Setup
{
    public abstract partial class SetupBase
    {
        protected Shop TimberComponentShop = new Shop(Guid.NewGuid(), "Shop to produce timber components");
        protected Shop MdfComponentShop = new Shop(Guid.NewGuid(), "Shop to produce MDF components");
        protected Shop GlassComponentShop = new Shop(Guid.NewGuid(), "Shop to produce glass components");
        protected Shop ProductAssemblyShop = new Shop(Guid.NewGuid(), "Shop to assemble doors");

        protected Shop MaterialStockpileShop = new Shop(Guid.NewGuid(), "Materials stockpile.");
        protected Shop ProductStockpileShop = new Shop(Guid.NewGuid(), "Products stockpile.");

        protected Shop ShopToRemove = new Shop(Guid.NewGuid(), "Shop to remove");
        protected HashSet<Shop> Shops;

        private void InitShops()
        {
            TimberComponentShop.ShopCategory = TimberComponentShopCategory;
            TimberComponentShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Material1Timber, 1M), TimberComponentShop);
            InventoryService.IncreaseInventory(new InventoryItem(Material2Foil, 2M), TimberComponentShop);
            TimberComponentShop.Manager = ManagerOfTimberComponentShop;

            MdfComponentShop.ShopCategory = MdfComponentShopCategory;
            MdfComponentShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Material3Mdf, 2M), MdfComponentShop);
            InventoryService.IncreaseInventory(new InventoryItem(Material2Foil, 2M), MdfComponentShop);
            MdfComponentShop.Manager = ManagerOfMdfComponentShop;

            GlassComponentShop.ShopCategory = GlassComponentShopCategory;
            GlassComponentShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Material4TintedGlass, 2M), GlassComponentShop);
            GlassComponentShop.Manager = ManagerOfGlassComponentShop;

            ProductAssemblyShop.ShopCategory = ProductAssemblyShopCategory;
            ProductAssemblyShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Component1Vertical, 10), ProductAssemblyShop);
            InventoryService.IncreaseInventory(new InventoryItem(Component2Horizontal, 10), ProductAssemblyShop);
            InventoryService.IncreaseInventory(new InventoryItem(Component3MdfFiller, 10), ProductAssemblyShop);
            InventoryService.IncreaseInventory(new InventoryItem(Component4GlassFiller, 10), ProductAssemblyShop);
            ProductAssemblyShop.Manager = ManagerOfProductAssemblyShop;

            MaterialStockpileShop.ShopCategory = MaterialStockpileShopCategory;
            MaterialStockpileShop.IsEntryPoint = true;
            MaterialStockpileShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Material1Timber, 100M), MaterialStockpileShop);
            InventoryService.IncreaseInventory(new InventoryItem(Material2Foil, 200M), MaterialStockpileShop);
            InventoryService.IncreaseInventory(new InventoryItem(Material3Mdf, 200M), MaterialStockpileShop);
            InventoryService.IncreaseInventory(new InventoryItem(Material4TintedGlass, 50M), MaterialStockpileShop);
            MaterialStockpileShop.Manager = ManagerOfMaterialstockPile;

            ProductStockpileShop.IsExitPoint = true;
            ProductStockpileShop.ShopCategory = ProductStockpileShopCategory;
            ProductStockpileShop.IsDefaultForCategory = true;
            InventoryService.IncreaseInventory(new InventoryItem(Product1InteriorDoor, 50M), ProductStockpileShop);
            ProductStockpileShop.Manager = ManagerOfProductstockPile;
            OrderService.AssignOrder(OrderReadyForShipment, ProductStockpileShop);

            ShopToRemove.Manager = ManagerOfProductAssemblyShop;
            ShopToRemove.ShopCategory = ShopCategoryToArchive;

            Shops = new HashSet<Shop>
            {
                TimberComponentShop, MdfComponentShop, GlassComponentShop,
                ProductAssemblyShop, MaterialStockpileShop, ProductStockpileShop
            };
        }

        private void StubShopsRepositories()
        {
            A.CallTo(() => ShopRepo.LoadAsync(ShopToRemove.Id)).Returns(ShopToRemove);
            A.CallTo(() => ShopRepo.GetAsync(ShopToRemove.Id)).Returns(ShopToRemove);

            A.CallTo(() => ShopRepo.LoadAsync(MaterialStockpileShop.Id)).Returns(MaterialStockpileShop);
            A.CallTo(() => ShopRepo.GetAsync(MaterialStockpileShop.Id)).Returns(MaterialStockpileShop);

            A.CallTo(() => ShopRepo.LoadAsync(ProductStockpileShop.Id)).Returns(ProductStockpileShop);
            A.CallTo(() => ShopRepo.GetAsync(ProductStockpileShop.Id)).Returns(ProductStockpileShop);

            A.CallTo(() => ShopRepo.LoadAsync(TimberComponentShop.Id)).Returns(TimberComponentShop);
            A.CallTo(() => ShopRepo.GetAsync(TimberComponentShop.Id)).Returns(TimberComponentShop);

            A.CallTo(() => ShopRepo.LoadAsync(MdfComponentShop.Id)).Returns(MdfComponentShop);
            A.CallTo(() => ShopRepo.GetAsync(MdfComponentShop.Id)).Returns(MdfComponentShop);

            A.CallTo(() => ShopRepo.LoadAsync(GlassComponentShop.Id)).Returns(GlassComponentShop);
            A.CallTo(() => ShopRepo.GetAsync(GlassComponentShop.Id)).Returns(GlassComponentShop);

            A.CallTo(() => ShopRepo.LoadAsync(ProductAssemblyShop.Id)).Returns(ProductAssemblyShop);
            A.CallTo(() => ShopRepo.GetAsync(ProductAssemblyShop.Id)).Returns(ProductAssemblyShop);

            A.CallTo(() => ShopRepo.GetAllAsync()).Returns(Shops.ToList());
            A.CallTo(() => ShopRepo.GetEntryPoint()).Returns(MaterialStockpileShop);
            A.CallTo(() => ShopRepo.GetExitPoint()).Returns(ProductStockpileShop);

            A.CallTo(() => EntityRepository.LoadAsync<Shop>(ShopToRemove.Id)).Returns(ShopToRemove);
            A.CallTo(() => EntityRepository.GetAsync<Shop>(ShopToRemove.Id)).Returns(ShopToRemove);
            A.CallTo(() => EntityRepository.LoadAsync<Shop>(MaterialStockpileShop.Id)).Returns(MaterialStockpileShop);

            A.CallTo(() => EntityRepository.GetAsync<Shop>(MaterialStockpileShop.Id)).Returns(MaterialStockpileShop);
            A.CallTo(() => EntityRepository.LoadAsync<Shop>(ProductStockpileShop.Id)).Returns(ProductStockpileShop);
            A.CallTo(() => EntityRepository.GetAsync<Shop>(ProductStockpileShop.Id)).Returns(ProductStockpileShop);

            A.CallTo(() => EntityRepository.LoadAsync<Shop>(TimberComponentShop.Id)).Returns(TimberComponentShop);
            A.CallTo(() => EntityRepository.GetAsync<Shop>(TimberComponentShop.Id)).Returns(TimberComponentShop);
            A.CallTo(() => EntityRepository.LoadAsync<Shop>(MdfComponentShop.Id)).Returns(MdfComponentShop);

            A.CallTo(() => EntityRepository.GetAsync<Shop>(MdfComponentShop.Id)).Returns(MdfComponentShop);
            A.CallTo(() => EntityRepository.LoadAsync<Shop>(GlassComponentShop.Id)).Returns(GlassComponentShop);
            A.CallTo(() => EntityRepository.GetAsync<Shop>(GlassComponentShop.Id)).Returns(GlassComponentShop);

            A.CallTo(() => EntityRepository.LoadAsync<Shop>(ProductAssemblyShop.Id)).Returns(ProductAssemblyShop);
            A.CallTo(() => EntityRepository.GetAsync<Shop>(ProductAssemblyShop.Id)).Returns(ProductAssemblyShop);
        }
    }
}
