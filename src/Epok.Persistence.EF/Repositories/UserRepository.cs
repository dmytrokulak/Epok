using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly DomainContext _dbContext;

        public UserRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<User> LoadAsync(Guid id)
            => await _dbContext.Users.SingleAsync(e => e.Id == id);

        public async Task<IList<User>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Users.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<User> GetAsync(Guid id)
            => await _dbContext.Users
                .Include(e => e.Shop)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<User>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Users
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Shop)
                .ToListAsync();

        public async Task<IList<User>> GetAllAsync()
            => await _dbContext.Users
                .Include(e => e.Shop)
                .ToListAsync();

        public async Task AddAsync(User entity)
            => await _dbContext.Users.AddAsync(entity);

        public async Task AddRangeAsync(User entities)
            => await _dbContext.Users.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Users.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Users.RemoveRange(await LoadSomeAsync(ids));
    }
}
