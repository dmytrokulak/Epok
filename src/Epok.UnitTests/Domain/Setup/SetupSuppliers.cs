using Epok.Domain.Inventory.Entities;
using Epok.Domain.Suppliers;
using Epok.Domain.Suppliers.Entities;
using FakeItEasy;
using System;
using System.Collections.Generic;

namespace Epok.UnitTests.Domain.Setup
{
    public abstract partial class SetupBase
    {
        protected Supplier Material1TimberSupplier = new Supplier(Guid.NewGuid(), "Timber supplier");
        protected Supplier SupplierToArchive = new Supplier(Guid.NewGuid(), "Supplier to archive");

        protected MaterialRequest Material1TimberMaterialRequest =
            new MaterialRequest(Guid.NewGuid(), "Material1TimberMaterialRequest");

        private void InitSuppliers()
        {
            Material1TimberSupplier.SuppliableArticles = new HashSet<Article>() {Material1Timber};
            Material1TimberSupplier.MaterialRequests = new HashSet<MaterialRequest>()
            {
                new MaterialRequest(Guid.NewGuid(), "Timber request")
                {
                    Status = MaterialRequestStatus.Submitted,
                    ItemsRequested = new List<InventoryItem>()
                    {
                        new InventoryItem(Material1Timber, 100M)
                    }
                }
            };

            Material1TimberMaterialRequest.Status = MaterialRequestStatus.Submitted;
            Material1TimberMaterialRequest.Supplier = Material1TimberSupplier;
            Material1TimberMaterialRequest.ItemsRequested = new List<InventoryItem>
            {
                new InventoryItem(Material1Timber, 30)
            };
        }

        private void StubSuppliersRepositories()
        {

            A.CallTo(() => SupplierRepo.LoadAsync(Material1TimberSupplier.Id)).Returns(Material1TimberSupplier);
            A.CallTo(() => SupplierRepo.GetAsync(Material1TimberSupplier.Id)).Returns(Material1TimberSupplier);

            A.CallTo(() => SupplierRepo.LoadAsync(SupplierToArchive.Id)).Returns(SupplierToArchive);
            A.CallTo(() => SupplierRepo.GetAsync(SupplierToArchive.Id)).Returns(SupplierToArchive);

            A.CallTo(() => MaterialRequestRepo.LoadAsync(Material1TimberMaterialRequest.Id))
                .Returns(Material1TimberMaterialRequest);
            A.CallTo(() => MaterialRequestRepo.GetAsync(Material1TimberMaterialRequest.Id))
                .Returns(Material1TimberMaterialRequest);

        }
    }
}
