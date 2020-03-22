using Epok.Core.Domain.Queries;

namespace Epok.Domain.Customers.Queries
{
    public class CustomersQuery : QueryBase
    {
        public CustomerType? FilterCustomerTypeExact { get; set; }
        public string FilterCountryExact { get; set; }
        public string FilterProvinceExact { get; set; }
        public string FilterCityExact { get; set; }
    }
}
