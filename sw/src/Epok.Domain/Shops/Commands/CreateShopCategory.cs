using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Creates a new shop category.
    /// </summary>
    public class CreateShopCategory : CreateEntityCommand
    {
        /// <summary>
        /// Workshop or warehouse.
        /// </summary>
        public ShopType ShopType { get; set; }

        /// <summary>
        /// Articles allowed in the shop.
        /// </summary>
        public IEnumerable<Guid> Articles { get; set; }
    }
}
