using Epok.Core.Domain.Entities;
using System;

namespace Epok.Core.Domain.Events
{
    /// <summary>
    /// Event in response to a command performed.
    /// </summary>
    [Serializable]
    public class DomainEvent<T> : EventBase where T : IEntity
    {
        public DomainEvent(T entity, Trigger trigger, Guid userId)
        {
            EntityId = entity.Id;
            Entity = entity;
            Trigger = trigger;
            RaisedBy = userId;
        }

        public Guid EntityId { get; }
        public T Entity { get; }
        public Trigger Trigger { get; }
    }
}
