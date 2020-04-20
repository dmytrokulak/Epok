using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;

namespace Epok.Domain.Users.Queries.Handlers
{
    public class UsersQueryHandler : IQueryHandler<UsersQuery, User>
    {
        private readonly IEntityRepository _repository;

        public UsersQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<User>> HandleAsync(UsersQuery query)
        {
            Expression<Func<User, bool>> predicate = user =>
                (query.FilterNameLike == null || user.Name.Contains(query.FilterNameLike)) &&
                (query.FilterEmailLike == null || user.Email.Contains(query.FilterEmailLike) &&
                 (query.FilterUserTypeExact == null || user.UserType == query.FilterUserTypeExact) &&
                 (query.FilterIsShopManagerExact == null || user.IsShopManager == query.FilterIsShopManagerExact));

            if(query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate, query.Skip, query.Take, query.OrderBy, query.OrderMode);
            return await _repository.GetSomeAsync(query.FilterIds, predicate, query.Skip, query.Take, query.OrderBy, query.OrderMode);
        }
    }
}
