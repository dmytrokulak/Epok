using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Registers new article in the system. 
    /// Creates a corresponding bill of material.
    /// Allows it in a shop category if production shop 
    /// category is specified.
    /// </summary>
    public class RegisterArticle : CreateEntityCommand
    {
        public ArticleType ArticleType { get; set; }
        public string Code { get; set; }
        public Guid UomId { get; set; }
        public Guid? ProductionShopCategoryId { get; set; }
        public TimeSpan? TimeToProduce { get; set; }
        public IEnumerable<(Guid articleId, decimal amount)> BomInput { get; set; }
        public decimal BomOutput { get; set; }
    }
}
