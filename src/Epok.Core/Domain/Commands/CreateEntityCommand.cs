using System;

namespace Epok.Core.Domain.Commands
{
    /// <summary>
    /// Base command for an entity creation.
    /// </summary>
    public abstract class CreateEntityCommand : CommandBase
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Entity name.
        /// </summary>
        public string Name { get; set; }
    }
}
