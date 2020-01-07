using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Archives customer i.e. makes customer no longer 
    /// available in the system except for historic reporting.
    /// </summary>
    public class ArchiveCustomer : ArchiveEntityCommand
    {
    }
}
