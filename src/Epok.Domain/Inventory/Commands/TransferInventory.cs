using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Transfers inventory between shops.
    /// Domain exception is thrown if lacking inventory to transfer
    /// or the target shop does not allow the article to be transferred.
    /// </summary>
    public class TransferInventory : CommandBase
    {
        public Guid ArticleId { get; set; }
        public decimal Amount { get; set; }
        public Guid SourceShopId { get; set; }
        public Guid TargetShopId { get; set; }
    }
}
