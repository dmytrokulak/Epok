using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Suppliers.Queries.Handlers
{
    public class MaterialRequestsQueryHandler : IQueryHandler<MaterialRequestsQuery, MaterialRequest>
    {
        private readonly IEntityRepository _repository;

        public MaterialRequestsQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<MaterialRequest>> HandleAsync(MaterialRequestsQuery query)
        {
            Expression<Func<MaterialRequest, bool>> predicate = materialRequests =>
                (query.FilterNameLike == null || materialRequests.Name.Contains(query.FilterNameLike)) &&
                (query.FilterSupplierIdExact == null || materialRequests.Supplier.Id == query.FilterSupplierIdExact);

            if(query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
