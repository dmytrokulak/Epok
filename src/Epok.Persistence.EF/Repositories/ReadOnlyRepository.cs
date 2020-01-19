using Epok.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class ReadOnlyRepository : IReadOnlyRepository
    {
        private readonly DomainContext _dbContext;

        public ReadOnlyRepository(DomainContext dbContext)
            => _dbContext = dbContext;
        public async Task<T> GetAsync<T>(Guid id) where T : EntityBase
        {
            var set = _dbContext.Set<T>();

            var props = typeof(T).GetProperties()
                .Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)
                            || typeof(ICollection<>).IsAssignableFrom(p.PropertyType));

            foreach (var prop in props)
                set.Include(prop.Name);

            return await set.FindAsync(id);
        }

        public async Task<IList<T>> GetSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase
        {
            var set = _dbContext.Set<T>();

            var props = typeof(T).GetProperties()
                .Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)
                            || typeof(ICollection<>).IsAssignableFrom(p.PropertyType));

            foreach (var prop in props)
                set.Include(prop.Name);

            return await set.Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        public async Task<T> LoadAsync<T>(Guid id) where T : EntityBase
            => await _dbContext.FindAsync<T>(id);

        public async Task<IList<T>> LoadSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase
            => await _dbContext.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }
}
