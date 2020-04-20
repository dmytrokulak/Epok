using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Suppliers.Entities;

namespace Epok.Domain.Suppliers.Queries.Handlers
{
    public class SuppliersQueryHandler : IQueryHandler<SuppliersQuery, Supplier>
    {
        private readonly IEntityRepository _repository;

        public SuppliersQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Supplier>> HandleAsync(SuppliersQuery query)
        {
            Expression<Func<Supplier, bool>> predicate = supplier =>
                (query.FilterNameLike == null || supplier.Name.Contains(query.FilterNameLike)) &&
                (query.FilterArticleIdExact == null || supplier.SuppliableArticles.Any(a => a.Id == query.FilterArticleIdExact)) &&
                (query.FilterCountryExact == null || supplier.ShippingAddress.Country == query.FilterCountryExact) &&
                (query.FilterProvinceExact == null || supplier.ShippingAddress.Province == query.FilterProvinceExact) &&
                (query.FilterCityExact == null || supplier.ShippingAddress.City == query.FilterCityExact);

            if(query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate, query.Skip, query.Take, query.OrderBy, query.OrderMode);
            return await _repository.GetSomeAsync(query.FilterIds, predicate, query.Skip, query.Take, query.OrderBy, query.OrderMode);
        }
    }
}
