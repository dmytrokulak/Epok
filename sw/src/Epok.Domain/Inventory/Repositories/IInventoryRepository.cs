using Epok.Domain.Inventory.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Inventory.Repositories
{
    public interface IInventoryRepository : IRepository<InventoryItem>
    {
        /// <summary>
        /// Finds total amount of inventory for specified article across all shops.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Task<decimal> FindTotalAmountInStockAsync(Article article);

        /// <summary>
        /// Inventory in stock and not in active orders.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Task<decimal> FindSpareInventoryAsync(Article article);

        /// <summary>
        /// Total amount of items ordered in active (not yet 
        /// shipped and not cancelled) orders.
        /// </summary>
        Task<decimal> FindTotalAmountInOrdersAsync(Article article);
    }
}
