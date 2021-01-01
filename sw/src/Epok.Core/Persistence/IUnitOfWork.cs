using System.Threading.Tasks;

namespace Epok.Core.Persistence
{
    /// <summary>
    /// Produces a scope for commands
    /// to be performed as a single transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
