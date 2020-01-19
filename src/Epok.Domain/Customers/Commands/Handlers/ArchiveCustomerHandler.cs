using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Customers.Entities;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;
using static Epok.Domain.Customers.ExceptionCauses;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Archives customer if they do not have active orders
    /// and transmits a corresponding event.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when customer have active i.e. not shipped orders.
    /// </exception>
    public class ArchiveCustomerHandler : ICommandHandler<ArchiveCustomer>
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveCustomerHandler(IRepository<Customer> customerRepo,
            IEventTransmitter eventTransmitter)
        {
            _customerRepo = customerRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveCustomer command)
        {
            var customer = await _customerRepo.GetAsync(command.Id);
            if (customer.Orders.Any(o => o.Status != Orders.OrderStatus.Shipped))
                throw new DomainException(ArchivingCustomerWithActiveOrders(customer));
            await _customerRepo.ArchiveAsync(customer.Id);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Customer>(customer, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
