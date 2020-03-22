using System;
using Epok.Core.Domain.Queries;

namespace Epok.Domain.Orders.Queries
{
    public class OrdersQuery : QueryBase
    {
        public OrderType? FilterTypeExact { get; set; }
        public OrderStatus? FilterStatusExact { get; set; }
        public Guid? FilterCustomerExact { get; set; }
    }
}
