using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Core.Persistence;
using System.Threading.Tasks;
using Epok.Domain.Contacts.Entities;
using static Epok.Domain.Contacts.ExceptionCauses;

namespace Epok.Domain.Contacts.Commands.Handlers
{
    /// <summary>
    /// Removes a contact from the system.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when contact is primary for a company.
    /// </exception>
    public class ArchiveContactHandler : ICommandHandler<ArchiveContact>
    {
        private readonly IEntityRepository _repository;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveContactHandler(IEntityRepository repository, IEventTransmitter eventTransmitter)
        {
            _repository = repository;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveContact command)
        {
            var contact = await _repository.LoadAsync<Contact>(command.Id);
            if (contact.Primary)
                throw new DomainException(ArchivingPrimaryContact(contact));
            await _repository.RemoveAsync(contact);

            await _eventTransmitter.BroadcastAsync(new DomainEvent<Contact>(contact, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
