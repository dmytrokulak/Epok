using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using Epok.Domain.Suppliers.Entities;
using System.Linq;
using System.Threading.Tasks;
using static Epok.Domain.Suppliers.ExceptionCauses;

namespace Epok.Domain.Suppliers.Commands.Handlers
{
    /// <summary>
    /// Archives a supplier.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown if supplier has active material requests.
    /// </exception>
    public class ArchiveSupplierHandler : ICommandHandler<ArchiveSupplier>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveSupplierHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveSupplier command)
        {
            var supplier = await _repository.GetAsync<Supplier>(command.Id);
            if (supplier.MaterialRequests.Any(r => r.Status != MaterialRequestStatus.Fulfilled))
                throw new DomainException(ArchivingSupplierWithActiveMaterialRequests(supplier));

            await _repository.RemoveAsync(supplier);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Supplier>(supplier, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
