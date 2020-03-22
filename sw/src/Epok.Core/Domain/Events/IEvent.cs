using System;

namespace Epok.Core.Domain.Events
{
    /// <summary>
    /// Events are messages raised in response to 
    /// successful handling of domain commands.
    /// </summary>
    public interface IEvent
    {
        DateTimeOffset RaisedAt { get; }
        Guid RaisedBy { get; }
    }
}