using Epok.Domain.Shops.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class ShopCategoryRepository : IRepository<ShopCategory>
    {
        private readonly DomainContext _dbContext;

        public ShopCategoryRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<ShopCategory> LoadAsync(Guid id)
            => await _dbContext.ShopCategories.SingleAsync(e => e.Id == id);

        public async Task<IList<ShopCategory>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.ShopCategories.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<ShopCategory> GetAsync(Guid id)
            => await _dbContext.ShopCategories
                .Include(e => e.Articles)
                .Include(e => e.Shops)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<ShopCategory>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.ShopCategories
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Articles)
                .Include(e => e.Shops)
                .ToListAsync();

        public async Task<IList<ShopCategory>> GetAllAsync()
            => await _dbContext.ShopCategories
                .Include(e => e.Articles)
                .Include(e => e.Shops)
                .ToListAsync();

        public async Task AddAsync(ShopCategory entity)
            => await _dbContext.ShopCategories.AddAsync(entity);

        public async Task AddRangeAsync(ShopCategory entities)
            => await _dbContext.ShopCategories.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.ShopCategories.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.ShopCategories.RemoveRange(await LoadSomeAsync(ids));

    }
}
