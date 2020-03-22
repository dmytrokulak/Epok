using System;
using Epok.Core.Domain.Queries;

namespace Epok.Domain.Inventory.Queries
{
    public class BillsOfMaterialQuery : QueryBase
    {
        public Guid? FilterArticleIdExact { get; set; }
    }
}
