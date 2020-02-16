using System.Threading.Tasks;

namespace Epok.Core.Domain.Events
{
    /// <summary>
    /// Transmits events via a message queue. 
    /// </summary>
    public interface IEventTransmitter
    {
        /// <summary>
        /// Transmits events to any subscribed services.
        /// Should be called from handlers only 
        /// </summary>
        Task BroadcastAsync<T>(T @event) where T : IEvent;

        /// <summary>
        /// Throws an exception if connection to message bus fails.
        /// </summary>
        void VerifyConnection();
        //ToDo:4 add HomeEnqueue to communicate messages between subdomains ? 
    }
}
