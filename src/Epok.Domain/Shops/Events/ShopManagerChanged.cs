using Epok.Core.Domain.Events;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Users.Entities;
using System;

namespace Epok.Domain.Shops.Events
{
    /// <summary>
    /// An event raised in response to a shop
    /// manager changed.
    /// </summary>
    [Serializable]
    public class ShopManagerChanged : DomainEvent<Shop>
    {
        public ShopManagerChanged(User dismissed, Shop entity, Trigger trigger, Guid userId)
            : base(entity, trigger, userId)
        {
            Dismissed = dismissed;
        }

        public User Dismissed { get; }
    }
}
