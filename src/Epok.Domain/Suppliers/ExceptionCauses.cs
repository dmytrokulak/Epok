using Epok.Domain.Suppliers.Entities;
using System;
using System.Linq;

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
    }
}
