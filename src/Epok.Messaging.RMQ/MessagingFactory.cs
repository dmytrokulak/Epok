using System;
using RabbitMQ.Client;

namespace Epok.Messaging.RMQ
{
    public class MessagingFactory : IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public MessagingFactory()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel WithQueue(string name)
        {
             _channel.QueueDeclare(name, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
             return _channel;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}