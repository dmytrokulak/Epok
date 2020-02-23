using Epok.Core.Domain.Queries;

namespace Epok.Domain.Customers.Queries
{
    public class CustomersQuery : QueryBase
    {
        public CustomerType FilterCustomerType { get; set; }
    }
}
