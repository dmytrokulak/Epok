using Epok.Domain.Suppliers.Entities;
using System;
using System.Linq;
using Epok.Domain.Inventory.Entities;

namespace Epok.Domain.Suppliers
{
    public static class ExceptionCauses
    {
        public static readonly Func<Supplier, string> ArchivingSupplierWithActiveMaterialRequests
            = supplier => $"Cannot archive supplier {supplier.Id} with active material requests " +
                          $@"{supplier.MaterialRequests.Where(r => r.Status != MaterialRequestStatus.Fulfilled)
                              .Select(r => r.Id.ToString()).Aggregate((cur, nxt) => $"{cur}, {nxt}")}.";

        public static readonly Func<Supplier, Guid, string> RequestingUnregisteredArticle
            = (supplier, articleId) => $"Supplier {supplier.Id} does not supply article {articleId}.";

        public static readonly Func<Article, Supplier, string> SupplyingFinishedProduct
            = (article, supplier) => $"Cannot add {article} as suppliable of {supplier} because" +
                                     $"article's type is {article.ArticleType} which meant to be manufactured.";
    }
}
