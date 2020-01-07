using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Creates a new bill of material for the article.
    /// Throws a domain exception if a bill of material
    /// with the same input exists for the article.
    /// </summary>
    public class AddBillOfMaterial : CreateEntityCommand
    {
        public Guid ArticleId { get; set; }
        public IEnumerable<(Guid articleId, decimal amount)> Input { get; set; }
        public decimal Output { get; set; }
    }
}
