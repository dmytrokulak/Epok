using Epok.Core.Domain.Entities;
using System;
using System.Linq;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Contains an article and its amount as a single entity.
    /// </summary>
    [Serializable]
    public class InventoryItem : EntityBase
    {
        //orm ctor
        protected InventoryItem()
        {
        }
        public InventoryItem(Article article, decimal amount)
            : base(Guid.NewGuid(), $"{article.Name} item.")
        {
            Article = article;
            Amount = amount;
        }

        public static InventoryItem Empty(Article article)
            => new InventoryItem(article, 0);

        public virtual Article Article { get; set; }
        public virtual decimal Amount { get; set; }
        //shop where exiting inventory is located
        public virtual Shop Shop { get; set; }
        //order where existing inventory is included
        public virtual Order Order { get; set; }

        public virtual TimeSpan TimeToProduce 
            => Article.ProductionShopCategory is null
                ? TimeSpan.Zero
                : TimeSpan.FromTicks((long) (Amount * Article.TimeToProduce.Value.Ticks));

        public virtual DateTimeOffset EarliestProductionStartTime
        {
            get
            {
                var eta = Article.ProductionShopCategory?.Shops
                    .SelectMany(s => s.Orders)
                    .Min(o => o?.EstimatedCompletionAt);
                if (eta is null || eta < DateTimeOffset.Now)
                    return DateTimeOffset.Now;
                return eta.Value;
            }
        }

        protected bool Equals(InventoryItem other)
        {
            return Article == other.Article && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InventoryItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Article.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
            => $"{Article.Name} {Amount} {Article.UoM.Name}";
    }
}
