using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Allows article to be stored
    /// in shops of the specified category.
    /// </summary>
    public class AllowArticle : CommandBase
    {
        public Guid ArticleId { get; set; }
        public Guid ShopCategoryId { get; set; }
    }
}
