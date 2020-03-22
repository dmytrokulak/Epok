using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Creates a report on the inventory items
    /// spoiled due to production defects.
    /// </summary>
    public class ReportSpoilage : CommandBase
    {
        public Guid ShopId { get; set; }
        public Guid ArticleId { get; set; }
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
        public bool Fixable { get; set; }
        public string Reason { get; set; }
    }
}
