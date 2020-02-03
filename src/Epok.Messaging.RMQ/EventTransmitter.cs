using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Epok.Core.Domain.Events;

namespace Epok.Messaging.RMQ
{
    public class EventTransmitter : IEventTransmitter
    {
        private readonly MessagingFactory _factory;

        public EventTransmitter(MessagingFactory factory)
        {
            _factory = factory;
        }

        public async Task BroadcastAsync<T>(T @event) where T : IEvent
        {
            try
            {
                await Task.Run(() =>
                {
                    byte[] data;
                    using (var stream = new MemoryStream())
                    {
                        new BinaryFormatter().Serialize(stream, @event);
                        data = stream.ToArray();
                    }

                    _factory.WithQueue("DomainEvents")
                        .BasicPublish(exchange: "", routingKey: "DomainEvents", mandatory: false,
                            basicProperties: null, body: data);
                });
            }
            catch (Exception)
            {
              //ToDo: Log exception at EventsEmitter
              //exception handling should probably be in a decorator 
            }
        }
    }
}
