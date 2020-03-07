using System;
using Epok.Core.Domain.Queries;

namespace Epok.Domain.Suppliers.Queries
{
    public class MaterialRequestsQuery : QueryBase
    {
        public Guid?  FilterSupplierIdExact { get; set; }
    }
}
