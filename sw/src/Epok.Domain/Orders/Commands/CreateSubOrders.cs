using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Orders.Commands
{
    /// <summary>
    /// Creates internal sub orders in shops.
    /// </summary>
    /// ToDo:3 merge with EnactOrder ?
    public class CreateSubOrders : CommandBase
    {
        public Guid OrderId { get; set; }
    }
}
