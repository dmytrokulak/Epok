using Epok.Core.Domain.Events;
using Epok.Domain.Orders.Entities;
using System;

namespace Epok.Domain.Orders.Events
{
    public class OrderEnacted : DomainEvent<Order>
    {
        public OrderEnacted(Order order, Guid userId)
            : base(order, Trigger.Changed, userId)
        {
        }
    }
}
