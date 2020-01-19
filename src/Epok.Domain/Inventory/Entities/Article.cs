using Epok.Core.Domain.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Represents any raw material, component or product
    /// which can be used in business operations: 
    /// procurement, storage, production and shipment.
    /// </summary>
    [Serializable]
    public class Article : EntityBase
    {
        public Article(Guid id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// Material, component, product etc
        /// </summary>
        public virtual ArticleType ArticleType { get; set; }

        /// <summary>
        /// Human readable code of this article.
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// Units of measurement.
        /// </summary>
        public virtual Uom UoM { get; set; }

        /// <summary>
        /// Calculation of input of other articles 
        /// required to produce specified amount of this article.
        /// </summary>
        public virtual HashSet<BillOfMaterial> BillsOfMaterial { get; set; } = new HashSet<BillOfMaterial>();

        public virtual BillOfMaterial PrimaryBillOfMaterial => BillsOfMaterial.SingleOrDefault(bom => bom.Primary);

        /// <summary>
        /// Shops category able to handle production of this article.
        /// </summary>

        public virtual ShopCategory ProductionShopCategory { get; set; }

        /// <summary>
        /// Nominal time required to produce a unit of this article.
        /// </summary>

        public virtual TimeSpan? TimeToProduce { get; set; }
    }
}
