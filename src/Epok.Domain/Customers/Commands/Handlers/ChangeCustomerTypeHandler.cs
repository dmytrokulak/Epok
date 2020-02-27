using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Customers.Entities;

namespace Epok.Domain.Customers.Commands.Handlers
{
    /// <summary>
    /// Changes customer's type.
    /// Throws an exception if NewCustomerType is undefined.
    /// </summary>
    public class ChangeCustomerTypeHandler : ICommandHandler<ChangeCustomerType>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ChangeCustomerTypeHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ChangeCustomerType command)
        {
            var customer = await _repository.GetAsync<Customer>(command.Id);

            if (command.NewCustomerType == CustomerType.Undefined)
                throw new DomainException(ExceptionCauses.SettingCustomerTypeToUndefined(customer));

            if (customer.CustomerType == command.NewCustomerType)
                return;
            
            customer.CustomerType = command.NewCustomerType;
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Customer>(customer, Trigger.Changed,
                command.InitiatorId));
        }
    }
}
