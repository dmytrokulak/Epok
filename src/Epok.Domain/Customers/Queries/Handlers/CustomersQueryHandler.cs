using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Customers.Entities;

namespace Epok.Domain.Customers.Queries.Handlers
{
    public class CustomersQueryHandler : IQueryHandler<CustomersQuery, Customer>
    {
        private readonly IEntityRepository _repository;

        public CustomersQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Customer>> HandleAsync(CustomersQuery query)
        {
            Expression<Func<Customer, bool>> predicate = customer =>
                query.FilterName == null || customer.Name.Contains(query.FilterName);

            if(query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
