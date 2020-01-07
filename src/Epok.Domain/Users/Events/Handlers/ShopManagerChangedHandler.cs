using Epok.Core.Domain.Events;
using Epok.Domain.Shops.Events;
using System;
using System.Threading.Tasks;

namespace Epok.Domain.Users.Events.Handlers
{
    public class ShopManagerChangedHandler : IEventHandler<ShopManagerChanged>
    {
        public Task HandleAsync(ShopManagerChanged @event)
        {
            //ToDo:4 assign permissions to new manager and remove from old one
            throw new NotImplementedException();
        }
    }
}
