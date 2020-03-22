using Epok.Core.Domain.Services;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System.Collections.Generic;

namespace Epok.Domain.Orders.Services
{
    public interface IOrderService : IDomainService
    {
        IEnumerable<InventoryItem> CalculateInventoryInput(Order order);

        /// <summary>
        /// Marks order as shipped.
        /// </summary>
        void ShipOrder(Order order);

        void AssignOrder(Order order, Shop shop);
        void RemoveOrder(Order order, Shop shop);
        void IncreaseProduced(InventoryItem item, Order order);
        bool IsReadyForShipment(Order order);
        Order CreateSubOrder(InventoryItem item, Order parent);
    }
}