using Epok.Core.Domain.Persistence;
using Epok.Domain.Shops.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Shops.Repositories
{
    public interface IShopRepository : IRepository<Shop>
    {
        /// <summary>
        /// Shop to supply materials to.
        /// </summary>
        /// <returns></returns>
        Task<Shop> GetEntryPoint();

        /// <summary>
        /// Shop to ship products from.
        /// </summary>
        /// <returns></returns>
        Task<Shop> GetExitPoint();
    }
}
