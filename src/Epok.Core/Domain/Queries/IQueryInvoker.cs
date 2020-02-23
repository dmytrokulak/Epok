using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Domain.Entities;

namespace Epok.Core.Domain.Queries
{
    /// <summary>
    /// Uses IoC container as a service locator to decide
    /// on the query handler to handle the query passed.
    /// Should have IoC container injected as dependency.
    /// </summary>
    public interface IQueryInvoker
    {
        Task<IList<TEntity>> Execute<TQuery, TEntity>(TQuery query) where TQuery : IQuery where TEntity : IEntity;
    }
}
