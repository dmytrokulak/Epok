using Epok.Core.Domain.Commands;

namespace Epok.Domain.Shops.Commands
{
    /// <summary>
    /// Archives the specified shop category.
    /// Domain exception is thrown if category
    /// contains shops.
    /// </summary>
    public class ArchiveShopCategory : ArchiveEntityCommand
    {
    }
}
