using Epok.Core.Domain.Entities;
using Epok.Domain.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Shops.Entities
{
    /// <summary>
    /// Shops grouping according to the articles allowed.
    /// </summary>
    [Serializable]
    public class ShopCategory : EntityBase
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public ShopCategory()
        {
        }
        public ShopCategory(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// Workshop or warehouse.
        /// </summary>
        public virtual ShopType ShopType { get; set; }

        public virtual HashSet<Shop> Shops { get; set; }
        //ToDO:2 save default (primary) props in db, not query
        public virtual Shop DefaultShop => Shops.SingleOrDefault(s => s.IsDefaultForCategory);

        /// <summary>
        /// Articles allowed in the shops.
        /// </summary>
        public virtual HashSet<Article> Articles { get; set; } = new HashSet<Article>();
    }
}
