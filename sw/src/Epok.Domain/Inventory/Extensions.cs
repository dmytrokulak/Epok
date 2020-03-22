using Epok.Domain.Inventory.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Inventory
{
    public static class Extensions
    {
        /// <summary>
        /// Of article.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="article"></param>
        /// <returns></returns>
        public static InventoryItem Of(this IEnumerable<InventoryItem> items, Article article)
            => items.SingleOrDefault(item => item.Article == article);
    }
}
