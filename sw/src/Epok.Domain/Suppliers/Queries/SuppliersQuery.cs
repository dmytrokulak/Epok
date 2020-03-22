using System;
using Epok.Core.Domain.Queries;

namespace Epok.Domain.Suppliers.Queries
{
    public class SuppliersQuery : QueryBase
    {
        public Guid?  FilterArticleIdExact { get; set; }
        public string FilterCountryExact { get; set; }
        public string FilterProvinceExact { get; set; }
        public string FilterCityExact { get; set; }
    }
}
