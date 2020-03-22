using Epok.Core.Domain.Services;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Domain.Exceptions;

namespace Epok.Domain.Inventory.Services
{
    /// <summary>
    /// Handles modifications on inventory entities.
    /// </summary>
    public interface IInventoryService : IDomainService
    {
        /// <summary>
        /// Handles production of an inventory item if it is allowed in the specified shop,
        /// included in the order, the amount requested to produce does not exceed that 
        /// in the order and if there is enough materials to handle the production.
        /// </summary>
        /// <param name="shop">Production shop</param>
        /// <param name="item"></param>
        /// <param name="order">Order for which production is performed</param>
        /// <exception cref="DomainException">
        /// Thrown if 1) article is not allowed in the shop 2) article is not specified by the order
        /// 3) amount requested for production exceeds the amount required for order to be fulfilled
        /// 4) there is insufficient materials and\or components in stock to produce the required amount.
        /// </exception>
        InventoryItem Produce(Shop shop, InventoryItem item, Order order);

        Task<InventoryItem> Produce(Shop shop, Guid articleId, decimal amount, Order order);

        /// <summary>
        /// Transfer specified amount of the specified article between the shops
        /// if the target shops allows for this article, there is enough 
        /// inventory in the source shop and the amount and assortment of the 
        /// inventory to transfer complies with the specified orders.
        /// </summary>
        /// <param name="source">Shop dispatching inventory</param>
        /// <param name="target">Shop receiving inventory</param>
        /// <param name="item">Inventory item</param>
        /// <exception cref="DomainException">
        /// Thrown if 1) article is not allowed in the target shop 2) article is not specified by 
        /// any bill of material of any article found in the specified orders or not present in such bom
        /// 3) there is insufficient materials and\or components in stock to produce the required amount.
        /// </exception>
        InventoryItem Transfer(Shop source, Shop target, InventoryItem item);

        /// <summary>
        /// Report components spoiled due to production defects
        /// </summary>
        InventoryItem ReportSpoilage(InventoryItem item, Order order, Shop shop);

        /// <summary>
        /// Report components spoiled due to production defects
        /// </summary>
        Task<InventoryItem> ReportSpoilage(Guid articleId, decimal amount, bool fixable, Order order, Shop shop);

        /// <summary>
        /// Increases shop's inventory of the specified
        /// article to the specified amount.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="shop"></param>
        void IncreaseInventory(InventoryItem item, Shop shop);

        /// <summary>
        /// Increases shops inventory of the specified
        /// articles to the specified amounts.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="shop"></param>
        void IncreaseInventory(IEnumerable<InventoryItem> items, Shop shop);

        /// <summary>
        /// Decreases shop's inventory of the specified
        /// article to the specified amount.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="shop"></param>
        void DecreaseInventory(InventoryItem item, Shop shop);

        /// <summary>
        /// Decreases shops inventory of the specified
        /// articles to the specified amounts.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="shop"></param>
        void DecreaseInventory(IEnumerable<InventoryItem> items, Shop shop);

        /// <summary>
        /// Calculates time of production completion of the specified
        /// items taking into account spare amount in stock, amount
        /// in items specified and time of production of each article.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<DateTimeOffset> CalculateTimeOfCompletion(IEnumerable<InventoryItem> items);

        /// <summary>
        /// Allocatable amount being amount in stock and possible
        /// to be produced from the materials in stock.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Task<decimal> CalculateAllocatableAmount(Article article);
    }
}