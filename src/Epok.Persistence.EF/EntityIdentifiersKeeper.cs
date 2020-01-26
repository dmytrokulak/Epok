using Epok.Core.Domain.Entities;
using Epok.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Epok.Core.Domain.Exceptions;

namespace Epok.Persistence.EF
{
    public class EntityIdentifiersKeeper : IEntityIdentifiersKeeper
    {
        private readonly Dictionary<Type, HashSet<Guid>> _idsKeeper;

        public EntityIdentifiersKeeper()
        {
            _idsKeeper = Assembly.Load("Epok.Domain").GetTypes()
                .Where(t => typeof(IEntity).IsAssignableFrom(t))
                .ToDictionary(t => t, t => new HashSet<Guid>());
        }

        public HashSet<Guid> Get<T>() where T : IEntity
            => _idsKeeper[typeof(T)];

        public void Update<T>(HashSet<Guid> ids) where T : IEntity
            => _idsKeeper[typeof(T)] = ids;

        public void Remove(HashSet<(Type Type, Guid Id)> toRemove)
        {
            foreach (var (type, id) in toRemove)
            {
                var key = type.Name.EndsWith("Proxy") ? type.BaseType : type;
                _idsKeeper[key].Remove(id);
            }
        }

        public void Add(HashSet<(Type Type, Guid Id)> toAdd)
        {
            foreach (var (type, id) in toAdd)
            {
                var key = type.Name.EndsWith("Proxy") ? type.BaseType : type;
                _idsKeeper[key].Add(id);
            }
        }
    }
}
