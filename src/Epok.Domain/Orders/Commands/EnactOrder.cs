using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Orders.Commands
{
    /// <summary>
    /// Passes order to workshops for manufacturing.
    /// </summary>
    public class EnactOrder : CommandBase
    {
        public Guid OrderId { get; set; }
    }
}
