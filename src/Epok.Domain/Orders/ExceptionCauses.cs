using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using System;

namespace Epok.Domain.Orders
{
    public static class ExceptionCauses
    {
        public static string InsufficientTime(string orderName, DateTimeOffset deadline, DateTimeOffset eta)
            => $"Not enough time to fulfill the order{orderName}. Shipment deadline {deadline}, eta {eta}.";

        public static string OrderNotAtExitPoint(Order order)
            => $"{order} is at {order.Shop} which is not an exit point.";

        public static string OrderNotContainArticle(Order order, Article article)
            => $"{order} does not contain {article} in ordered items.";

        public static string OrderOverflow(Order order, Article article, decimal input)
            => $"{order} contains provision for {order.ItemsOrdered.Of(article).Amount} of {article} " +
               $"with {order.ItemsProduced.Of(article).Amount} already produced. Attempt was made to increase items produced by {input}.";

        public static string OrderIsNotReadyForShipment(Order order)
            => $"{order} is not ready for shipment.";
    }
}
