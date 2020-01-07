using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Creates new shop and assigns a manager.
    /// </summary>
    public class CreateShop : CreateEntityCommand
    {
        public Guid ShopCategoryId { get; set; }

        /// <summary>
        /// Id of a user to be set as a manager.
        /// </summary>
        public Guid ManagerId { get; set; }

        /// <summary>
        /// Whether materials can be supplied 
        /// to this shop from an external supplier.
        /// </summary>
        public bool IsEntryPoint { get; set; }

        /// <summary>
        /// Whether products can be shipped
        /// to customers from this shop.
        /// </summary>
        public bool IsExitPoint { get; set; }

    }
}
