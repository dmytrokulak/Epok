using Epok.Core.Domain.Commands;

namespace Epok.Domain.Orders.Commands
{
    /// <summary>
    /// Ships order to customer.
    /// Domain exception is thrown if order is
    /// not ready for the shipment i.e. not all
    /// items are produces or order is
    /// not at the exit point.
    /// </summary>
    public class ShipOrder : ArchiveEntityCommand
    {
    }
}
