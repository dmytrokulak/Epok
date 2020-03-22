using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Modifies bill of material for an article.
    /// </summary>
    public class ChangeBillOfMaterial : CommandBase
    {
        public Guid Id { get; set; }
        public IEnumerable<(Guid articleId, decimal amount)> Input { get; set; }
        public decimal Output { get; set; }
    }
}
