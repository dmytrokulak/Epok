using Epok.Domain.Shops.Entities;
using Epok.Domain.Users.Entities;

namespace Epok.Domain.Shops
{
    public static class ExceptionCauses
    {
        public static string UserIsAlreadyManager(User user)
            => $"{user} is already assigned as manager to {user.Shop}";

        public static string SameManagerAssigned(User user)
            => $"Attempt to replace manager of {user.Shop} with the same {user}.";

        public static string ArchivingShopWithInventory(Shop shop)
            => $"There is inventory still in stock with {shop}.";

        public static string ArchivingShopCategoryWithShops(ShopCategory shopCategory)
            => $"There is shops still with {shopCategory}.";
    }
}
