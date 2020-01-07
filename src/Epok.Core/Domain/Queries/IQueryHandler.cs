
using System.Threading.Tasks;

namespace Epok.Core.Domain.Queries
{
    /// <summary>
    /// Marker interface for a domain query handler.
    /// </summary>
    public interface IQueryHandler
    {

    }

    public interface IQueryHandler<T> : IQueryHandler where T : IQuery
    {
        Task HandleAsync(T query);
    }
}
