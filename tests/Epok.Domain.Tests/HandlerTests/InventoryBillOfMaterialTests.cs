using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Commands.Handlers;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Tests.Setup;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Tests.HandlerTests
{
    [TestFixture]
    public class InventoryBillOfMaterialTests : SetupBase
    {
        [Test]
        public async Task ChangeBom_Success()
        {
            //adhere
            var initial = Product1InteriorDoor.PrimaryBillOfMaterial.DeepCopy();

            //arrange
            const decimal inputM1 = 3;
            const decimal inputM2 = 5;
            const decimal inputM4 = 4;

            var command = new ChangeBillOfMaterial()
            {
                Id = Product1InteriorDoor.PrimaryBillOfMaterial.Id,
                Input = new HashSet<(Guid, decimal)>
                {
                    (Material1Timber.Id, inputM1),
                    (Material2Foil.Id, inputM2),
                    (Material4TintedGlass.Id, inputM4)
                },
                Output = 1,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ChangeBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var events = GetRecordedEvents<DomainEvent<BillOfMaterial>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(Product1InteriorDoor.PrimaryBillOfMaterial));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Changed));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            var bom = events[0].Entity;
            Assert.That(bom.Input.Of(Material1Timber).Amount, Is.EqualTo(inputM1));
            Assert.That(bom.Input.Of(Material1Timber).Amount,
                Is.Not.EqualTo(initial.Input.Of(Material1Timber)?.Amount));
            Assert.That(bom.Input.Of(Material2Foil).Amount, Is.EqualTo(inputM2));
            Assert.That(bom.Input.Of(Material2Foil).Amount, Is.Not.EqualTo(initial.Input.Of(Material2Foil)?.Amount));
            Assert.That(bom.Input.Of(Material4TintedGlass).Amount, Is.EqualTo(inputM4));
            Assert.That(bom.Input.Of(Material4TintedGlass).Amount,
                Is.Not.EqualTo(initial.Input.Of(Material4TintedGlass)?.Amount));

            //annul
            Product1InteriorDoor.PrimaryBillOfMaterial.Input = initial.Input;
            Product1InteriorDoor.PrimaryBillOfMaterial.Output = initial.Output;
        }

        [Test]
        public async Task AddBom_Success()
        {
            const decimal amountC1 = 2;
            const decimal amountC2 = 2;
            const decimal amountC3 = 2;
            const decimal amountM2 = 2;

            //adhere
            var initial = Product1InteriorDoor.BillsOfMaterial.DeepCopy();

            //arrange
            var command = new AddBillOfMaterial()
            {
                ArticleId = Product1InteriorDoor.Id,
                Name = "new bom for P1",
                Input = new HashSet<(Guid, decimal)>
                {
                    (Component1Vertical.Id, amountC1),
                    (Component2Horizontal.Id, amountC2),
                    (Component3MdfFiller.Id, amountC3),
                    (Material2Foil.Id, amountM2),
                },
                Output = 1,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new AddBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.AddAsync)), Is.EqualTo(1));
            var bom = GetRecordedEntities<BillOfMaterial>(EntityRepository, nameof(EntityRepository.AddAsync)).Single();
            Assert.That(bom.Name, Is.EqualTo(command.Name));
            Assert.That(bom.Article, Is.EqualTo(Product1InteriorDoor));
            Assert.That(bom.Output, Is.EqualTo(command.Output));
            Assert.That(bom.Input.Of(Component1Vertical).Amount, Is.EqualTo(amountC1));
            Assert.That(bom.Input.Of(Component2Horizontal).Amount, Is.EqualTo(amountC2));
            Assert.That(bom.Input.Of(Component3MdfFiller).Amount, Is.EqualTo(amountC3));
            Assert.That(bom.Input.Of(Material2Foil).Amount, Is.EqualTo(amountM2));
            Assert.That(bom.Primary, Is.EqualTo(false));

            var events = GetRecordedEvents<DomainEvent<BillOfMaterial>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(bom));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Added));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            var bomsDiff = Product1InteriorDoor.BillsOfMaterial.Except(initial).SingleOrDefault();
            Assert.That(bomsDiff, Is.EqualTo(bom));

            //annul
            Product1InteriorDoor.BillsOfMaterial = initial;
        }

        [Test]
        public void AddBom_FailFor_IdenticalBomExists()
        {
            //arrange
            var bom = Product1InteriorDoor.BillsOfMaterial.First();
            var command = new AddBillOfMaterial()
            {
                ArticleId = Product1InteriorDoor.Id,
                Name = "new bom for P1",
                Input = bom.Input.Select(i => (i.Article.Id, i.Amount)),
                Output = 1,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new AddBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(IdenticalBomExists(bom)));
            Assert.That(GetRecordedEvents<DomainEvent<BillOfMaterial>>(), Is.Empty);
        }

        [Test]
        public async Task ArchiveBom_Success()
        {
            //adhere
            var initial = Component2Horizontal.BillsOfMaterial.DeepCopy();

            //arrange
            var bom = Component2Horizontal.PrimaryBillOfMaterial;
            var command = new ArchiveBillOfMaterial()
            {
                Id = bom.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            Assert.That(CallsTo(EntityRepository, nameof(EntityRepository.ArchiveAsync)), Is.EqualTo(1));
            var id = GetRecordedIds(EntityRepository, nameof(EntityRepository.ArchiveAsync)).Single();
            Assert.That(id, Is.EqualTo(bom.Id));

            var events = GetRecordedEvents<DomainEvent<BillOfMaterial>>();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0].Entity, Is.EqualTo(bom));
            Assert.That(events[0].Trigger, Is.EqualTo(Trigger.Removed));
            Assert.That(events[0].RaisedBy, Is.EqualTo(command.InitiatorId));

            var bomDiff = initial.Except(Component2Horizontal.BillsOfMaterial).SingleOrDefault();
            Assert.That(bomDiff, Is.EqualTo(bom));
            Assert.That(Component2Horizontal.PrimaryBillOfMaterial, Is.Not.Null);
            Assert.That(Component2Horizontal.PrimaryBillOfMaterial, Is.Not.EqualTo(bom));

            //annul
            Component2Horizontal.BillsOfMaterial = initial;

        }

        [Test]
        public void ArchiveBom_FailFor_TheOnlyBomForProducibleArticle()
        {
            //arrange
            var bom = Product1InteriorDoor.PrimaryBillOfMaterial;
            var command = new ArchiveBillOfMaterial()
            {
                Id = bom.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new ArchiveBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(ArchivingTheOnlyBomForProducibleArticle(bom)));
            Assert.That(GetRecordedEvents<DomainEvent<BillOfMaterial>>(), Is.Empty);
        }

        [Test]
        public async Task SetPrimaryBom_Success()
        {
            //adhere
            var initial = Component2Horizontal.BillsOfMaterial.DeepCopy();

            //arrange
            var bom = Component2Horizontal.BillsOfMaterial.Single(b => !b.Primary);
            var command = new SetPrimaryBillOfMaterial()
            {
                BomId = bom.Id,
                ArticleId = Component2Horizontal.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new SetPrimaryBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //act
            await handler.HandleAsync(command);

            //assert
            var events = GetRecordedEvents<DomainEvent<BillOfMaterial>>();
            Assert.That(events.Count, Is.EqualTo(2));
            Assert.That(events.Select(e => e.Entity), Is.EquivalentTo(Component2Horizontal.BillsOfMaterial));
            Assert.That(events.All(e => e.Trigger == Trigger.Changed), Is.True);
            Assert.That(events.All(e => e.RaisedBy == command.InitiatorId), Is.True);
            Assert.That(Component2Horizontal.PrimaryBillOfMaterial, Is.EqualTo(bom));

            //annul
            Component2Horizontal.BillsOfMaterial = initial;
        }

        [Test]
        public void SetPrimaryBom_FailFor_BomIsAlreadyPrimary()
        {
            //arrange
            var command = new SetPrimaryBillOfMaterial()
            {
                BomId = Component2Horizontal.PrimaryBillOfMaterial.Id,
                ArticleId = Component2Horizontal.Id,
                InitiatorId = GlobalAdmin.Id
            };
            var handler = new SetPrimaryBillOfMaterialHandler(EntityRepository, EventTransmitter);

            //assert () => act
            var ex = Assert.ThrowsAsync<DomainException>(async () => await handler.HandleAsync(command));
            Assert.That(ex.Message, Is.EqualTo(BomIsAlreadyPrimary(Component2Horizontal.PrimaryBillOfMaterial)));
            Assert.That(GetRecordedEvents<DomainEvent<BillOfMaterial>>(), Is.Empty);
        }
    }
}
