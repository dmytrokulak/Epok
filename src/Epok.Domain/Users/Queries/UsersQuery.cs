using Epok.Core.Domain.Queries;

namespace Epok.Domain.Users.Queries
{
    public class UsersQuery : QueryBase
    {
        public UserType? FilterUserTypeExact { get; set; }
        public string FilterEmailLike { get; set; }
        public bool? FilterIsShopManagerExact { get; set; }
    }
}
