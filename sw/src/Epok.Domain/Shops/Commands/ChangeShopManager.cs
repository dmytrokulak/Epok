using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Changes a manager of the specified shop.
    /// Domain exception is thrown if the user
    /// specified is already a shop manager.
    /// </summary>
    public class ChangeShopManager : CommandBase
    {
        public Guid ShopId { get; set; }
        public Guid NewManagerId { get; set; }
    }
}
