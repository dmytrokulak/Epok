using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;

namespace Epok.Domain.Inventory.Queries.Handlers
{
    public class UomQueryHandler : IQueryHandler<UomQuery, Uom>
    {
        private readonly IEntityRepository _repository;

        public UomQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Uom>> HandleAsync(UomQuery query)
        {
            Expression<Func<Uom, bool>> predicate = uom =>
                (query.FilterNameLike == null || uom.Name.Contains(query.FilterNameLike)) &&
                (query.FilterTypeExact == null || uom.Type == query.FilterTypeExact);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
