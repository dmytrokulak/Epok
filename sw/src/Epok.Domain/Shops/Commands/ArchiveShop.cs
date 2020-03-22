using Epok.Core.Domain.Commands;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Archives the specified shop.
    /// Domain exception is thrown if shop has inventory.
    /// </summary>
    public class ArchiveShop : ArchiveEntityCommand
    {
    }
}
