using System.Threading.Tasks;

namespace Epok.Core.Domain.Queries
{
    /// <summary>
    /// Uses IoC container as a service locator to decide
    /// on the query handler to handle the query passed.
    /// Should have IoC container injected as dependency.
    /// </summary>
    public interface IQueryInvoker
    {
        Task Execute<T>(T query) where T : IQuery;
    }
}
