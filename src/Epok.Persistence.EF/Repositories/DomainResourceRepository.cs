using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class DomainResourceRepository : IRepository<DomainResource>
    {
        private readonly DomainContext _dbContext;

        public DomainResourceRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<DomainResource> LoadAsync(Guid id)
            => await _dbContext.DomainResources.SingleAsync(e => e.Id == id);

        public async Task<IList<DomainResource>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.DomainResources.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<DomainResource> GetAsync(Guid id)
            => await _dbContext.DomainResources.SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<DomainResource>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.DomainResources.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<IList<DomainResource>> GetAllAsync()
            => await _dbContext.DomainResources.ToListAsync();

        public async Task AddAsync(DomainResource entity)
            => await _dbContext.DomainResources.AddAsync(entity);

        public async Task AddRangeAsync(DomainResource entities)
            => await _dbContext.DomainResources.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.DomainResources.Remove(await GetAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.DomainResources.RemoveRange(await GetSomeAsync(ids));
    }
}