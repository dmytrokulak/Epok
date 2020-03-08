using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Orders.Entities;

namespace Epok.Domain.Orders.Queries.Handlers
{
    public class OrdersQueryHandler : IQueryHandler<OrdersQuery, Order>
    {
        private readonly IEntityRepository _repository;

        public OrdersQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Order>> HandleAsync(OrdersQuery query)
        {
            Expression<Func<Order, bool>> predicate = order =>
                (query.FilterNameLike == null || order.Name.Contains(query.FilterNameLike)) &&
                (query.FilterTypeExact == null || order.Type == query.FilterTypeExact) &&
                (query.FilterStatusExact == null || order.Status == query.FilterStatusExact) &&
                (query.FilterCustomerExact == null || order.Customer.Id == query.FilterCustomerExact);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
