using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;

namespace Epok.Domain.Shops.Queries.Handlers
{
    public class ShopCategoriesQueryHandler : IQueryHandler<ShopCategoriesQuery, ShopCategory>
    {
        private readonly IEntityRepository _repository;

        public ShopCategoriesQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<ShopCategory>> HandleAsync(ShopCategoriesQuery query)
        {
            Expression<Func<ShopCategory, bool>> predicate = category =>
                (query.FilterNameLike == null || category.Name.Contains(query.FilterNameLike)) &&
                (query.FilterShopTypeExact == null || category.ShopType == query.FilterShopTypeExact);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
