using Epok.Core.Domain.Events;
using Epok.Domain.Orders.Entities;
using System;

namespace Epok.Domain.Orders.Events
{
    public class OrderShipped : DomainEvent<Order>
    {
        public OrderShipped(Order order, Guid userId)
            : base(order, Trigger.Changed, userId)
        {
        }
    }
}
