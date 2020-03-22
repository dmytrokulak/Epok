using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Orders.Commands
{
    /// <summary>
    /// Creates a new external order.
    /// Domain exception is thrown if materials in stock
    /// is not enough to produce the items ordered
    /// or estimated time to of production exceeds the
    /// shipment deadline.
    /// </summary>
    public class CreateOrder : CreateEntityCommand
    {
        public Guid CustomerId { get; set; }
        public IEnumerable<(Guid ArticleId, decimal Amount)> Items { get; set; }
        public DateTimeOffset ShipmentDeadline { get; set; }
        public OrderType OrderType { get; set; }
    }
}
