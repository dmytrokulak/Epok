using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Epok.Core.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Epok.Integration.Tests
{
    internal static class RmqClient
    {
        private const string QueueName = "DomainEvents";
        private static readonly IModel Channel;
        private static readonly BinaryFormatter Formatter;
        private static readonly EventingBasicConsumer Consumer;
        private static readonly object SyncLock = new object();
        static RmqClient()
        {
            Formatter = new BinaryFormatter();
            Channel = new ConnectionFactory {HostName = "localhost"}.CreateConnection().CreateModel();
            Consumer = new EventingBasicConsumer(FromQueue());
        }

        private static IModel FromQueue()
        {
            Channel.QueueDeclare(QueueName, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            return Channel;
        }


        public static T Consume<T>() where T : EventBase
        {
            lock (SyncLock)
            {
                var sw = new Stopwatch();
                sw.Start();

                T consumedEvent = null;
               
                Consumer.Received += ConsumerOnReceived;

                FromQueue().BasicConsume(QueueName, true, Consumer);
                while (consumedEvent == null && sw.Elapsed < TimeSpan.FromSeconds(10))
                    Thread.Sleep(10);

                Consumer.Received -= ConsumerOnReceived;
                return consumedEvent;

                void ConsumerOnReceived(object? model, BasicDeliverEventArgs ea)
                {
                    using var stream = new MemoryStream(ea.Body);
                    consumedEvent = Formatter.Deserialize(stream) as T;
                    if (consumedEvent == null)
                        Broadcast(consumedEvent);
                }
            }
        }

        private static void Broadcast<T>(T @event) where T : IEvent
        {
            using var stream = new MemoryStream();

            Formatter.Serialize(stream, @event);

            FromQueue().BasicPublish(exchange: "", routingKey: QueueName, mandatory: false,
                    basicProperties: null, body: stream.ToArray());
        }
    }
}