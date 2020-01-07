using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Repositories
{
    public interface IInventoryRepository : IRepository<Article>
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
        /// Finds a spoiled article which corresponds to the specified article.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="fixable"></param>
        /// <returns></returns>
        Task<SpoiledArticle> FindSpoiledCounterpartAsync(Article article, bool fixable);
    }
}
