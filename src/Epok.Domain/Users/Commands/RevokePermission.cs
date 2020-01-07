using Epok.Core.Domain.Commands;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Revokes permission from a user on a CQRS resource.
    /// </summary>
    public class RevokePermission : ArchiveEntityCommand
    {
    }
}
