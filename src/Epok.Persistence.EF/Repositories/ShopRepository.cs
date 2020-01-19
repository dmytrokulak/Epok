using Epok.Domain.Shops.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class ShopRepository : IRepository<Shop>
    {
        private readonly DomainContext _dbContext;

        public ShopRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Shop> LoadAsync(Guid id)
            => await _dbContext.Shops.SingleAsync(e => e.Id == id);

        public async Task<IList<Shop>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Shops.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Shop> GetAsync(Guid id)
            => await _dbContext.Shops
                .Include(e => e.Inventory)
                .Include(e => e.Manager)
                .Include(e => e.Orders)
                .Include(e => e.ShopCategory)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Shop>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Shops
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Inventory)
                .Include(e => e.Manager)
                .Include(e => e.Orders)
                .Include(e => e.ShopCategory)
                .ToListAsync();

        public async Task<IList<Shop>> GetAllAsync()
            => await _dbContext.Shops
                .Include(e => e.Inventory)
                .Include(e => e.Manager)
                .Include(e => e.Orders)
                .Include(e => e.ShopCategory)
                .ToListAsync();

        public async Task AddAsync(Shop entity)
            => await _dbContext.Shops.AddAsync(entity);

        public async Task AddRangeAsync(Shop entities)
            => await _dbContext.Shops.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Shops.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Shops.RemoveRange(await LoadSomeAsync(ids));

    }
}
