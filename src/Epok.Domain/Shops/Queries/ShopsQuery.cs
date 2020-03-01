using Epok.Core.Domain.Queries;
using System;

namespace Epok.Domain.Shops.Queries
{
    public class ShopsQuery : QueryBase
    {
        public Guid? FilterShopCategoryExact { get; set; }
    }
}
