using Epok.Core.Domain.Entities;
using Epok.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        private readonly DomainContext _dbContext;
        private readonly IEntityIdentifiersKeeper _idsKeeper;
        private static Dictionary<Type, List<string>> _includes;

        public EntityRepository(DomainContext dbContext, IEntityIdentifiersKeeper idsKeeper)
        {
            _dbContext = dbContext;
            _idsKeeper = idsKeeper;
            PopulateIncludes();
        }

        private static void PopulateIncludes()
        {
            _includes ??= Assembly.Load("Epok.Domain").GetTypes().Where(IsEntity)
                .ToDictionary(t => t, t => t.GetProperties()
                    .Where(p => p.CanWrite && (IsEntity(p.PropertyType) || IsEntities(p.PropertyType)))
                    .Select(p => p.Name).ToList());

            bool IsEntity(Type t) => typeof(IEntity).IsAssignableFrom(t);

            bool IsEntities(Type t) => typeof(IEnumerable).IsAssignableFrom(t)
                                       && t.IsGenericType && t.GenericTypeArguments.Length == 1
                                       && IsEntity(t.GenericTypeArguments[0]);
        }


        public async Task<T> LoadAsync<T>(Guid id) where T : EntityBase
            => await _dbContext.Set<T>().FindAsync(id);

        public async Task<IList<T>> LoadSomeAsync<T>(IEnumerable<Guid> ids = null, 
            Expression<Func<T, bool>> predicate = null) where T : EntityBase
        {
            var tracked = _dbContext.ChangeTracker.Entries<T>()
                .Where(e => ids == null || ids.Contains(e.Entity.Id))
                .Where(e => predicate == null || predicate.Compile()(e.Entity))
                .Select(e => e.Entity)
                .ToList();

            var idsToTrack = ids?.Except(tracked.Select(e => e.Id)).ToList();
            var toTrack = new List<T>();
            if (idsToTrack?.Count != 0)
            {
                var set = _dbContext.Set<T>().AsQueryable()
                    .Where(e => idsToTrack.Contains(e.Id));
                if (predicate != null)
                    set = set.Where(predicate);
                toTrack = await set.ToListAsync();
            }

            tracked.AddRange(toTrack);
            return tracked;
        }

        public async Task<IList<T>> LoadAllAsync<T>() where T : EntityBase
        {
            var trackedIdsWithOutAdded = _dbContext.ChangeTracker
                .Entries<T>()
                .Where(e => e.State != EntityState.Added)
                .Select(e => e.Entity.Id).ToHashSet();

            var tracked = _dbContext.ChangeTracker
                .Entries<T>()
                .Select(e => e.Entity).ToList();

            if (trackedIdsWithOutAdded.Count != 0
                && _idsKeeper.Get<T>().SetEquals(trackedIdsWithOutAdded))
                return tracked;

            var toTrack = await _dbContext.Set<T>()
                .Where(c => !trackedIdsWithOutAdded.Contains(c.Id))
                .ToListAsync();
            tracked.AddRange(toTrack);
            _idsKeeper.Update<T>(tracked.Select(e => e.Id).ToHashSet());
            return tracked;
        }

        public async Task<T> GetAsync<T>(Guid id) where T : EntityBase
        {
            var set = _dbContext.Set<T>().AsQueryable();

            foreach (var propName in _includes[typeof(T)])
                set = set.Include(propName);

            return await set.SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IList<T>> GetSomeAsync<T>(IEnumerable<Guid> ids = null, 
            Expression<Func<T, bool>> predicate = null) where T : EntityBase
        {
            var set = _dbContext.Set<T>().AsQueryable();

            foreach (var propName in _includes[typeof(T)])
                set = set.Include(propName);

            if (ids != null)
                set = set.Where(e => ids.Contains(e.Id));
            if(predicate != null)
                set = set.Where(predicate);

            return await set.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync<T>() where T : EntityBase
        {
            var set = _dbContext.Set<T>().AsQueryable();

            foreach (var propName in _includes[typeof(T)])
                set = set.Include(propName);

            var entities = await set.ToListAsync();
            _idsKeeper.Update<T>(entities.Select(e => e.Id).ToHashSet());
            return entities;
        }

        public async Task AddAsync<T>(T entity) where T : EntityBase
            => await _dbContext.AddAsync(entity);

        public async Task AddSomeAsync<T>(T entities) where T : EntityBase
            => await _dbContext.AddRangeAsync(entities);

        public async Task RemoveAsync<T>(T entity) where T : EntityBase
            => await Task.Run(() => _dbContext.Remove(entity));

        public async Task RemoveSomeAsync<T>(IEnumerable<T> entities) where T : EntityBase
            => await Task.Run(() => _dbContext.RemoveRange(entities));
    }
}
