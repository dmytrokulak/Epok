using System;

namespace Epok.Core.Domain.Commands
{
    /// <summary>
    /// Base command for an entity archiving.
    /// </summary>
    public abstract class ArchiveEntityCommand : CommandBase
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        public Guid Id { get; set; }
    }
}
