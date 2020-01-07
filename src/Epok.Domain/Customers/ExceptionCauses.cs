using Epok.Core.Utilities;
using Epok.Domain.Customers.Entities;
using System.Linq;

namespace Epok.Domain.Customers
{
    public static class ExceptionCauses
    {
        public static string ArchivingCustomerWithActiveOrders(Customer customer)
            => $"Cannot archive {customer} with active orders: " +
               $"{customer.Orders.Where(o => o.Status != Orders.OrderStatus.Shipped).ToText()}.";
    }
}
