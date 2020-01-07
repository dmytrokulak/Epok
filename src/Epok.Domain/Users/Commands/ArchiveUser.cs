using Epok.Core.Domain.Commands;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Archives a user. A domain exception is
    /// thrown if user is a shop manager.
    /// </summary>
    public class ArchiveUser : ArchiveEntityCommand
    {
    }
}
