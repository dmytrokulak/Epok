using Epok.Core.Domain.Commands;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Archives a bill of material for the article.
    /// Thrown a domain exception when the bill of
    /// material to archive  is the only one left for
    /// the article which is intended for production.
    /// </summary>
    public class ArchiveBillOfMaterial : ArchiveEntityCommand
    {
    }
}
