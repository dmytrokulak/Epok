using System;
using Epok.Core.Domain.Queries;

namespace Epok.Domain.Inventory.Queries
{
    public class ArticlesQuery : QueryBase
    {
        public ArticleType? FilterArticleTypeExact { get; set; }
        public Guid? FilterUomExact { get; set; }
        public string FilterCodeLike { get; set; }
/* ToDo:2 Includes?
        public bool IncludeBillsOfMaterial { get; set; }
        public bool IncludeShopCategory { get; set; }*/
    }
}
