using Epok.Core.Domain.Commands;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Archives a supplier.
    /// A domain exception is thrown if the supplier
    /// has active material requests assigned.
    /// </summary>
    public class ArchiveSupplier : ArchiveEntityCommand
    {
    }
}
