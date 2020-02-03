using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Epok.Core.Domain.Entities;
using Epok.Core.Domain.Events;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Epok.Messaging.RMQ.Tests
{
    [TestFixture]
    [Timeout(1 * 60 * 1000)]
    public class MessagingTests
    {
        private MessagingFactory _rmq; 

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            try
            {
                _rmq = new MessagingFactory();
            }
            catch (Exception ex)
            {
                Assert.Ignore($"Cannot connect to RMQ server. {ex.Message}.");
            }
        }

        [Test]
        public async Task ShouldPublishAndConsumeEvents()
        {
            var formatter = new BinaryFormatter();

            var emitter = new EventTransmitter(_rmq);
            var domainEvent = new DomainEvent<DummyEntity>(new DummyEntity {Id = Guid.NewGuid()},
                Trigger.Added, Guid.NewGuid());
            await emitter.BroadcastAsync(domainEvent);

            var awaiting = true;
            DomainEvent<DummyEntity> consumedEvent = null;
            var consumer = new EventingBasicConsumer(_rmq.WithQueue("DomainEvents"));
            consumer.Received += (model, ea) =>
            {
                using var stream = new MemoryStream(ea.Body);
                consumedEvent = (DomainEvent<DummyEntity>) formatter.Deserialize(stream);
                awaiting = false;
            };

            _rmq.WithQueue("DomainEvents").BasicConsume("DomainEvents", true, consumer);
            while (awaiting)
            {
            }

            Assert.That(consumedEvent.EntityId, Is.EqualTo(domainEvent.EntityId));
        }
    }

    [Serializable]
    public class DummyEntity : EntityBase
    {
    }
}
