using System;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Article spoiled due to production defects.
    /// </summary>
    [Serializable]
    public class SpoiledArticle : Article
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public SpoiledArticle()
        {
        }
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

        public virtual Article Article { get; set; }

        /// <summary>
        /// Can be fixed into an article.
        /// </summary>
        public virtual bool Fixable { get; set; }

        /// <summary>
        /// Can be reused as an input in a bom.
        /// </summary>
        public virtual bool Reusable { get; set; }
    }
}
