using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Domain.Orders.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Total amount of items ordered in active (not yet 
        /// shipped and not cancelled) orders.
        /// </summary>
        Task<decimal> FindTotalAmountInOrdersAsync(Article article);

        /// <summary>
        /// Finds active orders assigned to shop.
        /// </summary>
        Task<IEnumerable<Order>> FindByShop(Shop shop);

        /// <summary>
        /// Finds active orders assigned to shops of this category.
        /// </summary>
        Task<IEnumerable<Order>> FindByShopCategory(ShopCategory shopCategory);
    }
}
