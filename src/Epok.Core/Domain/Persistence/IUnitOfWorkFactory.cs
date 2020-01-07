using System;

namespace Epok.Core.Domain.Persistence
{
    /// <summary>
    /// Produces a scope for commands
    /// to be performed as a single transaction.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        IDisposable Open();
    }
}
