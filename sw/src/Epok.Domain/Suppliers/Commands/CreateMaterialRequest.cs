using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Creates a new material request with the supplier.
    /// Domain exception is thrown if the supplier does
    /// not supplier the articles requested.
    /// </summary>
    public class CreateMaterialRequest : CreateEntityCommand
    {
        public Guid SupplierId { get; set; }
        public IEnumerable<(Guid ArticleId, decimal Amount)> Items { get; set; }
    }
}
