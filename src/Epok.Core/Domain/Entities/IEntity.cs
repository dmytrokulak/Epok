using System;

namespace Epok.Core.Domain.Entities
{
    /// <summary>
    /// Domain object that has a long term identity
    /// unrelated to a particular object instance.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Entity name.
        /// </summary>
        string Name { get; set; }
    }
}
