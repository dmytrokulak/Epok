using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Suppliers;
using Epok.Domain.Suppliers.Commands;
using Epok.Domain.Suppliers.Commands.Handlers;
using Epok.Domain.Suppliers.Entities;
using Epok.Domain.Suppliers.Events;
using Epok.UnitTests.Domain.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Domain.Inventory;
using static Epok.Domain.Suppliers.ExceptionCauses;

namespace Epok.UnitTests.Domain.HandlerTests
{
    [TestFixture]
    public class SupplierTests : SetupBase
    {
        [Test]
        public async Task RegisterSupplier_Success()
        {
            //arrange 
            var command = new RegisterSupplier()
            {
                Name = "Pvc Foil supplier",
                SuppliableArticleIds = new List<Guid> {Material2Foil.Id},
                PrimaryContactFirstName = "John",
                PrimaryContactLastName = "Smith",
                PrimaryContactPhone = "381111111111",
                PrimaryContactEmail = "john.smith@mail.me",
                ShippingAddressLine1 = "2 Paper st.",
                ShippingAddressLine2 = "2 Apt.",
                ShippingAddressCity = "Kyiv",
                ShippingAddressProvince = "Kyiv",
                ShippingAddressCountry = "Ukraine",
                ShippingAddressPostalCode = "37008",
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new RegisterSupplierHandler(SupplierRepo, ArticleRepo, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var entities = GetRecordedEntities(SupplierRepo, nameof(SupplierRepo.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].Name, Is.EqualTo(command.Name));
            Assert.That(entities[0].SuppliableArticles.Count, Is.EqualTo(1));
            Assert.That(entities[0].SuppliableArticles.Single(), Is.EqualTo(Material2Foil));
            Assert.That(entities[0].PrimaryContact.FirstName, Is.EqualTo(command.PrimaryContactFirstName));
            Assert.That(entities[0].PrimaryContact.LastName, Is.EqualTo(command.PrimaryContactLastName));
            Assert.That(entities[0].PrimaryContact.PhoneNumber, Is.EqualTo(command.PrimaryContactPhone));
            Assert.That(entities[0].PrimaryContact.Email, Is.EqualTo(command.PrimaryContactEmail));
            Assert.That(entities[0].ShippingAddress.AddressLine1, Is.EqualTo(command.ShippingAddressLine1));
            Assert.That(entities[0].ShippingAddress.AddressLine2, Is.EqualTo(command.ShippingAddressLine2));
            Assert.That(entities[0].ShippingAddress.City, Is.EqualTo(command.ShippingAddressCity));
            Assert.That(entities[0].ShippingAddress.Province, Is.EqualTo(command.ShippingAddressProvince));
            Assert.That(entities[0].ShippingAddress.Country, Is.EqualTo(command.ShippingAddressCountry));
            Assert.That(entities[0].ShippingAddress.PostalCode, Is.EqualTo(command.ShippingAddressPostalCode));

            var events = GetRecordedEvents<DomainEvent<Supplier>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public async Task ArchiveSupplier_Success()
        {
            //arrange
            var command = new ArchiveSupplier
            {
                Id = SupplierToArchive.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveSupplierHandler(SupplierRepo, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var ids = GetRecordedIds(SupplierRepo, nameof(SupplierRepo.ArchiveAsync));
            Assert.That(ids.Count, Is.EqualTo(1));
            Assert.That(ids[0], Is.EqualTo(command.Id));

            var events = GetRecordedEvents<DomainEvent<Supplier>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].Entity, Is.EqualTo(SupplierToArchive));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));
        }

        [Test]
        public void ArchiveSupplier_FailFor_ActiveMaterialRequests()
        {
            //arrange
            var command = new ArchiveSupplier
            {
                Id = Material1TimberSupplier.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveSupplierHandler(SupplierRepo, EventTransmitter);

            //arrange () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingSupplierWithActiveMaterialRequests(Material1TimberSupplier)));
            Assert.That(GetRecordedEvents<DomainEvent<Supplier>>(), Is.Empty);
        }

        [Test]
        public async Task CreateMaterialRequest_Success()
        {
            //adhere
            var initialRequests = Material1TimberSupplier.MaterialRequests.DeepCopy();

            //arrange
            const decimal amount = 50;
            var command = new CreateMaterialRequest()
            {
                Name = "New timber supplier",
                SupplierId = Material1TimberSupplier.Id,
                Items = new List<(Guid, decimal)> {(Material1Timber.Id, amount)},
                InitiatorId = GlobalAdmin.Id
            };
            var handler =
                new CreateMaterialRequestHandler(MaterialRequestRepo, ArticleRepo, SupplierRepo, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(Material1TimberSupplier.MaterialRequests.Count, Is.EqualTo(initialRequests.Count + 1));

            var entities = GetRecordedEntities(MaterialRequestRepo, nameof(MaterialRequestRepo.AddAsync));
            Assert.That(entities.Count, Is.EqualTo(1));
            Assert.That(entities[0].ItemsRequested.Count, Is.EqualTo(1));
            Assert.That(entities[0].ItemsRequested[0].Article, Is.EqualTo(Material1Timber));
            Assert.That(entities[0].ItemsRequested[0].Amount, Is.EqualTo(amount));
            Assert.That(entities[0].Status, Is.EqualTo(MaterialRequestStatus.Submitted));
            Assert.That(entities[0].Supplier, Is.EqualTo(Material1TimberSupplier));

            var events = GetRecordedEvents<DomainEvent<MaterialRequest>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].Entity, Is.EqualTo(entities[0]));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            //annul
            Material1TimberSupplier.MaterialRequests = initialRequests;
        }

        [Test]
        public void CreateMaterialRequest_FailFor_ArticleNotRegisteredWithSupplier()
        {
            //arrange
            var command = new CreateMaterialRequest()
            {
                SupplierId = SupplierToArchive.Id,
                Items = new List<(Guid, decimal)> {(Material1Timber.Id, 50M)},
                InitiatorId = GlobalAdmin.Id
            };
            var handler =
                new CreateMaterialRequestHandler(MaterialRequestRepo, ArticleRepo, SupplierRepo, EventTransmitter);

            //assert ()=> act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(RequestingUnregisteredArticle(SupplierToArchive, Material1Timber.Id)));
            Assert.That(GetRecordedEvents<DomainEvent<MaterialRequest>>(), Is.Empty);
        }

        [Test]
        public async Task ReceiveMaterials_Success()
        {
            //adhere
            var initialMaterialRequestStatus = Material1TimberMaterialRequest.Status;
            var initialShopInventory = MaterialStockpileShop.Inventory.DeepCopy();

            //arrange
            var command = new ReceiveMaterials
            {
                MaterialRequestId = Material1TimberMaterialRequest.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler =
                new ReceiveMaterialsHandler(MaterialRequestRepo, ShopRepo, InventoryService, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var diffInventory = MaterialStockpileShop.Inventory.Of(Material1Timber).Amount
                                - initialShopInventory.Of(Material1Timber).Amount;
            Assert.That(diffInventory,
                Is.EqualTo(Material1TimberMaterialRequest.ItemsRequested.Of(Material1Timber).Amount));
            Assert.That(initialMaterialRequestStatus, Is.EqualTo(MaterialRequestStatus.Submitted));
            Assert.That(Material1TimberMaterialRequest.Status, Is.EqualTo(MaterialRequestStatus.Fulfilled));
            Assert.That(GetRecordedEvents<MaterialsReceived>().Count, Is.EqualTo(1));

            //annul
            Material1TimberMaterialRequest.Status = initialMaterialRequestStatus;
            MaterialStockpileShop.Inventory = initialShopInventory;
        }
    }
}