using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Domain.Entities;
using Epok.Core.Domain.Queries;

namespace Epok.Composition.Invokers
{
    public class QueryInvoker : IQueryInvoker
    {
        public async Task<IList<TEntity>> Execute<TQuery, TEntity>(TQuery query)
            where TQuery : IQuery where TEntity : IEntity
        {
            var handler = Root.Container.GetInstance<IQueryHandler<TQuery, TEntity>>();
            return await handler.HandleAsync(query);
        }
    }
}
