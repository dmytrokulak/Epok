using System.Threading.Tasks;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;

namespace Epok.Domain.Inventory.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        /// <summary>
        /// Finds a spoiled article which corresponds to the specified article.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="fixable"></param>
        /// <returns></returns>
        Task<SpoiledArticle> FindSpoiledCounterpartAsync(Article article, bool fixable);
    }
}
