using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Sets a primary (default) bill of material for the article.
    /// A domain exception is thrown if the bill of material
    /// is already being set as primary.
    /// </summary>
    public class SetPrimaryBillOfMaterial : CommandBase
    {
        public Guid BomId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
