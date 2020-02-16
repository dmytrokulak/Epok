using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;

namespace Epok.Composition.Invokers
{
    public class QueryInvoker : IQueryInvoker
    {
        public async Task Execute<T>(T query) where T : IQuery
        {
            var handler = Root.Container.GetInstance<IQueryHandler<T>>();
            await handler.HandleAsync(query);
        }

        public async Task Execute<T>(IEnumerable<T> queries) where T : IQuery
        {
            foreach (var query in queries)
            {
                var handler = Root.Container.GetInstance<IQueryHandler<T>>();
                await handler.HandleAsync(query);
            }
        }
    }
}
