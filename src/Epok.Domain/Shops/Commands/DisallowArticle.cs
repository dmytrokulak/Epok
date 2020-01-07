using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Disallows an article to be stored
    /// in shops of the specified category.
    /// </summary>
    public class DisallowArticle : CommandBase
    {
        public Guid ArticleId { get; set; }
        public Guid ShopCategoryId { get; set; }
    }
}
