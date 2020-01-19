using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DomainContext _dbContext;

        public PermissionRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Permission> LoadAsync(Guid id)
            => await _dbContext.Permissions.SingleAsync(e => e.Id == id);

        public async Task<IList<Permission>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Permissions.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Permission> GetAsync(Guid id)
            => await _dbContext.Permissions
                .Include(e => e.User)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Permission>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Permissions
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.User)
                .ToListAsync();

        public async Task<IList<Permission>> GetAllAsync()
            => await _dbContext.Permissions
                .Include(e => e.User)
                .ToListAsync();

        public async Task AddAsync(Permission entity)
            => await _dbContext.Permissions.AddAsync(entity);

        public async Task AddRangeAsync(Permission entities)
            => await _dbContext.Permissions.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Permissions.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Permissions.RemoveRange(await LoadSomeAsync(ids));

        public async Task<Permission> Find(User user, DomainResource resource)
            => await _dbContext.Permissions
                .SingleOrDefaultAsync(p => p.User == user && p.Resource == resource);
    }
}
