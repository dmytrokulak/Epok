using Epok.Core.Domain.Commands;

namespace Epok.Domain.Inventory.Commands
{
    /// <summary>
    /// Archives article i.e. makes article no longer 
    /// available in the system except for historic reporting.
    /// A domain exception is thrown if article is still
    /// in stock or in active orders.
    /// </summary>
    public class ArchiveArticle : ArchiveEntityCommand
    {
    }
}
