using Epok.Domain.Users.Entities;
using FakeItEasy;
using System;

namespace Epok.Domain.Tests.Setup
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

        protected DomainResource ProduceInventoryItemHandler =
            new DomainResource(Guid.NewGuid(), "ProduceInventoryItemHandler");

        protected Permission UserWithPermissionsOnProduceInventoryItemHandler
            = new Permission(Guid.NewGuid(), "UserWithPermissionsOnProduceInventoryItemHandler");

        protected Permission GlobalAdminOnProduceInventoryItemHandler
            = new Permission(Guid.NewGuid(), "GlobalAdminOnProduceInventoryItemHandler");

        private void InitUsers()
        {
            GlobalAdmin.UserType = Epok.Domain.Users.UserType.GlobalAdmin;

            UserWithPermissionsOnProduceInventoryItemHandler.User = UserWithPermissions;
            UserWithPermissionsOnProduceInventoryItemHandler.Resource = ProduceInventoryItemHandler;

            GlobalAdminOnProduceInventoryItemHandler.User = GlobalAdmin;
            GlobalAdminOnProduceInventoryItemHandler.Resource = ProduceInventoryItemHandler;

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
            A.CallTo(() => EntityRepository.LoadAsync<User>(ManagerOfProductAssemblyShop.Id)).Returns(ManagerOfProductAssemblyShop);
            A.CallTo(() => EntityRepository.GetAsync<User>(ManagerOfProductAssemblyShop.Id)).Returns(ManagerOfProductAssemblyShop);

            A.CallTo(() => EntityRepository.LoadAsync<User>(UserToArchive.Id)).Returns(UserToArchive);
            A.CallTo(() => EntityRepository.GetAsync<User>(UserToArchive.Id)).Returns(UserToArchive);

            A.CallTo(() => EntityRepository.LoadAsync<User>(UserWithPermissions.Id)).Returns(UserWithPermissions);
            A.CallTo(() => EntityRepository.GetAsync<User>(UserWithPermissions.Id)).Returns(UserWithPermissions);

            A.CallTo(() => EntityRepository.LoadAsync<User>(GlobalAdmin.Id)).Returns(GlobalAdmin);
            A.CallTo(() => EntityRepository.GetAsync<User>(GlobalAdmin.Id)).Returns(GlobalAdmin);

            A.CallTo(() => EntityRepository.LoadAsync<DomainResource>(ProduceInventoryItemHandler.Id)).Returns(ProduceInventoryItemHandler);
            A.CallTo(() => EntityRepository.GetAsync<DomainResource>(ProduceInventoryItemHandler.Id)).Returns(ProduceInventoryItemHandler);

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

            A.CallTo(() => EntityRepository.GetAsync<Permission>(GlobalAdminOnProduceInventoryItemHandler.Id))
                .Returns(GlobalAdminOnProduceInventoryItemHandler);
            A.CallTo(() => EntityRepository.GetAsync<Permission>(UserWithPermissionsOnProduceInventoryItemHandler.Id))
                .Returns(UserWithPermissionsOnProduceInventoryItemHandler);
        }
    }
}
