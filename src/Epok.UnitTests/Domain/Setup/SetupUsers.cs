using Epok.Domain.Users.Entities;
using FakeItEasy;
using System;

namespace Epok.UnitTests.Domain.Setup
{
    public abstract partial class SetupBase
    {
        protected User ManagerOfProductAssemblyShop = new User(Guid.NewGuid(), "Shop Manager");

        protected User ManagerOfTimberComponentShop =
            new User(Guid.NewGuid(), "Shop Manager: ManagerOfTimberComponentsShop");

        protected User ManagerOfMdfComponentShop = new User(Guid.NewGuid(), "Shop Manager: ManagerOfMdfComponentsShop");

        protected User ManagerOfGlassComponentShop =
            new User(Guid.NewGuid(), "Shop Manager: ManagerOfGlassComponentShop");

        protected User ManagerOfProductstockPile = new User(Guid.NewGuid(), "Shop Manager: ManagerOfProductStockPile");

        protected User ManagerOfMaterialstockPile =
            new User(Guid.NewGuid(), "Shop Manager: ManagerOfMaterialStockPile");

        protected User UserToArchive = new User(Guid.NewGuid(), "User to archive");
        protected User UserWithPermissions = new User(Guid.NewGuid(), "User with permissions");

        protected User GlobalAdmin = new User(Guid.NewGuid(), "GlobalAdmin");

        protected CqrsResource ProduceInventoryItemHandler =
            new CqrsResource(Guid.NewGuid(), "ProduceInventoryItemHandler");

        protected Permission UserWithPermissionsOnProduceInventoryItemHandler
            = new Permission(Guid.NewGuid(), "UserWithPermissionsOnProduceInventoryItemHandler");

        protected Permission GlobalAdminOnProduceInventoryItemHandler
            = new Permission(Guid.NewGuid(), "GlobalAdminOnProduceInventoryItemHandler");

        private void InitUsers()
        {
            GlobalAdmin.UserType = Epok.Domain.Users.UserType.GlobalAdmin;

            UserWithPermissionsOnProduceInventoryItemHandler.User = UserWithPermissions;
            UserWithPermissionsOnProduceInventoryItemHandler.Handler = ProduceInventoryItemHandler;

            GlobalAdminOnProduceInventoryItemHandler.User = GlobalAdmin;
            GlobalAdminOnProduceInventoryItemHandler.Handler = ProduceInventoryItemHandler;

            ManagerOfProductAssemblyShop.IsShopManager = true;
            ManagerOfProductAssemblyShop.Shop = ProductAssemblyShop;
            ProductAssemblyShop.Manager = ManagerOfProductAssemblyShop;

            ManagerOfTimberComponentShop.IsShopManager = true;
            TimberComponentShop.Manager = ManagerOfTimberComponentShop;
            ManagerOfTimberComponentShop.Shop = TimberComponentShop;

            ManagerOfMdfComponentShop.IsShopManager = true;
            MdfComponentShop.Manager = ManagerOfMdfComponentShop;
            ManagerOfMdfComponentShop.Shop = MdfComponentShop;

            ManagerOfGlassComponentShop.IsShopManager = true;
            GlassComponentShop.Manager = ManagerOfGlassComponentShop;
            ManagerOfGlassComponentShop.Shop = GlassComponentShop;

            ManagerOfProductstockPile.IsShopManager = true;
            ProductStockpileShop.Manager = ManagerOfProductstockPile;
            ManagerOfProductstockPile.Shop = ProductStockpileShop;

            ManagerOfMaterialstockPile.IsShopManager = true;
            MaterialStockpileShop.Manager = ManagerOfMaterialstockPile;
            ManagerOfMaterialstockPile.Shop = MaterialStockpileShop;
        }

        private void StubUsersRepositories()
        {
            A.CallTo(() => UserRepo.LoadAsync(ManagerOfProductAssemblyShop.Id)).Returns(ManagerOfProductAssemblyShop);
            A.CallTo(() => UserRepo.GetAsync(ManagerOfProductAssemblyShop.Id)).Returns(ManagerOfProductAssemblyShop);

            A.CallTo(() => UserRepo.LoadAsync(UserToArchive.Id)).Returns(UserToArchive);
            A.CallTo(() => UserRepo.GetAsync(UserToArchive.Id)).Returns(UserToArchive);

            A.CallTo(() => UserRepo.LoadAsync(UserWithPermissions.Id)).Returns(UserWithPermissions);
            A.CallTo(() => UserRepo.GetAsync(UserWithPermissions.Id)).Returns(UserWithPermissions);

            A.CallTo(() => UserRepo.LoadAsync(GlobalAdmin.Id)).Returns(GlobalAdmin);
            A.CallTo(() => UserRepo.GetAsync(GlobalAdmin.Id)).Returns(GlobalAdmin);

            A.CallTo(() => HandlerRepo.LoadAsync(ProduceInventoryItemHandler.Id)).Returns(ProduceInventoryItemHandler);
            A.CallTo(() => HandlerRepo.GetAsync(ProduceInventoryItemHandler.Id)).Returns(ProduceInventoryItemHandler);

            A.CallTo(() => PermissionRepo.Find(ManagerOfProductAssemblyShop, ProduceInventoryItemHandler))
                .Returns(default(Permission));
            A.CallTo(() => PermissionRepo.Find(UserWithPermissions, ProduceInventoryItemHandler))
                .Returns(UserWithPermissionsOnProduceInventoryItemHandler);
            A.CallTo(() => PermissionRepo.Find(GlobalAdmin, ProduceInventoryItemHandler))
                .Returns(GlobalAdminOnProduceInventoryItemHandler);

            A.CallTo(() => PermissionRepo.GetAsync(GlobalAdminOnProduceInventoryItemHandler.Id))
                .Returns(GlobalAdminOnProduceInventoryItemHandler);
            A.CallTo(() => PermissionRepo.GetAsync(UserWithPermissionsOnProduceInventoryItemHandler.Id))
                .Returns(UserWithPermissionsOnProduceInventoryItemHandler);
        }
    }
}
