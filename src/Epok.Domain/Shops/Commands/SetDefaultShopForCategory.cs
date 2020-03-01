using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Sets shop as default for the category.
    /// </summary>
    public class SetDefaultShopForCategory : CommandBase
    {
        public Guid ShopCategoryId { get; set; }
        public Guid ShopId { get; set; }
    }
}
