using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;

namespace Epok.Domain.Inventory.Queries.Handlers
{
    public class BillsOfMaterialQueryHandler : IQueryHandler<BillsOfMaterialQuery, BillOfMaterial>
    {
        private readonly IEntityRepository _repository;

        public BillsOfMaterialQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<BillOfMaterial>> HandleAsync(BillsOfMaterialQuery query)
        {
            Expression<Func<BillOfMaterial, bool>> predicate = bom =>
                (query.FilterNameLike == null || bom.Name.Contains(query.FilterNameLike))
                && (query.FilterArticleIdExact == null || bom.Article.Id == query.FilterArticleIdExact);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
