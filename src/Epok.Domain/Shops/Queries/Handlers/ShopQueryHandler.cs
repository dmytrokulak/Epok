using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;

namespace Epok.Domain.Shops.Queries.Handlers
{
    public class ShopQueryHandler : IQueryHandler<ShopsQuery, Shop>
    {
        private readonly IEntityRepository _repository;

        public ShopQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Shop>> HandleAsync(ShopsQuery query)
        {
            Expression<Func<Shop, bool>> predicate = shop =>
                (query.FilterNameLike == null || shop.Name.Contains(query.FilterNameLike)) &&
                (query.FilterShopCategoryExact == null || shop.ShopCategory.Id == query.FilterShopCategoryExact);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
