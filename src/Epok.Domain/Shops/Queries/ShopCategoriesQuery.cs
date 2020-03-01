using Epok.Core.Domain.Queries;

namespace Epok.Domain.Shops.Queries
{
    public class ShopCategoriesQuery : QueryBase
    {
        public ShopType? FilterShopTypeExact { get; set; }
    }
}
