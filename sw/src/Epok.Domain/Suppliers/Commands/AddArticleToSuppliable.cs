using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Adds an article to the collection of suppliable
    /// by the given supplier.
    /// </summary>
    public class AddArticleToSuppliable : CommandBase
    {
        public Guid SupplierId { get; set; }
        public Guid ArticleId { get; set; }
    }
}
