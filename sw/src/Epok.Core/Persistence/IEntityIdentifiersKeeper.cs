using System;
using System.Collections.Generic;
using Epok.Core.Domain.Entities;

namespace Epok.Core.Persistence
{
    /// <summary>
    /// Keeps in memory the ids of all entities by type of an entity.
    /// Used to decide whether to query db when loading all entities
    /// or they all can be loaded from memory cache.
    /// </summary>
    public interface IEntityIdentifiersKeeper
    {
        /// <summary>
        /// Gets ids stored in keeper for this entity type.
        /// </summary>
        HashSet<Guid> Get<T>() where T : IEntity;
        /// <summary>
        /// Overwrites ids collection stored in keeper for this entity type.
        /// </summary>
        void Update<T>(HashSet<Guid> ids) where T : IEntity;
        /// <summary>
        /// Removes ids stored in keeper for these entity types.
        /// </summary>
        void Remove(HashSet<(Type Type, Guid Id)> toRemove);
        /// <summary>
        /// Adds ids to keeper for these entity types.
        /// </summary>
        void Add(HashSet<(Type Type, Guid Id)> toAdd);
    }
}