using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Produces inventory item according to its article's bill of material.
    /// Domain exception is thrown if materials are lacking for production,
    /// order does not have provision for the amount requested to be produced
    /// or the shop's category does not allow the article in its shops.
    /// </summary>
    public class ProduceInventoryItem : CommandBase
    {
        public Guid ShopId { get; set; }
        public Guid ArticleId { get; set; }
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
    }
}
