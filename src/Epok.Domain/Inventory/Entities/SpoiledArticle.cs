using System;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Article spoiled due to production defects.
    /// </summary>
    [Serializable]
    public class SpoiledArticle : Article
    {
        public SpoiledArticle(Guid id, string name) : base(id, name)
        {
        }

        public SpoiledArticle(Article article, bool fixable, bool reusable)
            : base(Guid.NewGuid(), $"{article.Name} spoiled {(fixable ? "" : "not")} fixable.")
        {
            Article = article;
            Fixable = fixable;
            Reusable = reusable;
        }

        public Article Article { get; set; }

        /// <summary>
        /// Can be fixed into an article.
        /// </summary>
        public bool Fixable { get; set; }

        /// <summary>
        /// Can be reused as an input in a bom.
        /// </summary>
        public bool Reusable { get; set; }
    }
}
