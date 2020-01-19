using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Inventory.Entities;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Archives a bill of material for the article.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when the bill of material to archive
    /// is the only one left for the article
    /// and this article is intended for production.
    /// </exception>
    public class ArchiveBillOfMaterialHandler : ICommandHandler<ArchiveBillOfMaterial>
    {
        private readonly IRepository<BillOfMaterial> _bomRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveBillOfMaterialHandler(IRepository<BillOfMaterial> bomRepo, IEventTransmitter eventTransmitter)
        {
            _bomRepo = bomRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveBillOfMaterial command)
        {
            var bom = await _bomRepo.LoadAsync(command.Id);
            if (bom.Article.BillsOfMaterial.Count == 1)
                throw new DomainException(ArchivingTheOnlyBomForProducibleArticle(bom));

            bom.Article.BillsOfMaterial.Remove(bom);
            if (bom.Primary)
                bom.Article.BillsOfMaterial.First().Primary = true;

            await _bomRepo.ArchiveAsync(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<BillOfMaterial>(bom, Trigger.Removed,
                command.InitiatorId));
        }
    }
}
