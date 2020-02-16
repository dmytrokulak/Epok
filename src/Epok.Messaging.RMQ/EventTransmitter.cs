using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Epok.Core.Domain.Events;
using RabbitMQ.Client;

namespace Epok.Messaging.RMQ
{
    public class EventTransmitter : IEventTransmitter
    {
        private ConnectionFactory _factory;
        private IModel _channel;
        private IConnection _connection;

        //ToDo:4 fishy stuff. public member not in interface. Used to be on now deleted MessagingFactory.
        public IModel WithQueue(string name)
        {
            _factory ??= new ConnectionFactory { HostName = "localhost" };
            _connection ??= _factory.CreateConnection();
            _channel ??= _connection.CreateModel();

            _channel.QueueDeclare(name, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            return _channel;
        }

        public void VerifyConnection()
        {
            _factory ??= new ConnectionFactory { HostName = "localhost" };
            _connection ??= _factory.CreateConnection();
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

                    WithQueue("DomainEvents")
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
