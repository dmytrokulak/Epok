using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Removes an article from the collection of suppliable
    /// by the given supplier.
    /// </summary>
    public class RemoveArticleFromSuppliable : CommandBase
    {
        public Guid SupplierId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
