using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DomainContext _dbContext;

        public InventoryRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<InventoryItem> LoadAsync(Guid id)
            => await _dbContext.InventoryItems.SingleAsync(e => e.Id == id);

        public async Task<IList<InventoryItem>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.InventoryItems.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<InventoryItem> GetAsync(Guid id)
            => await _dbContext.InventoryItems
                .Include(e => e.Article)
                .Include(e => e.Shop)
                .Include(e => e.Order)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<InventoryItem>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.InventoryItems
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Article)
                .Include(e => e.Shop)
                .Include(e => e.Order)
                .ToListAsync();

        public async Task<IList<InventoryItem>> GetAllAsync()
            => await _dbContext.InventoryItems
                .Include(e => e.Article)
                .Include(e => e.Shop)
                .Include(e => e.Order)
                .ToListAsync();

        public async Task AddAsync(InventoryItem entity)
            => await _dbContext.InventoryItems.AddAsync(entity);

        public async Task AddRangeAsync(InventoryItem entities)
            => await _dbContext.InventoryItems.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.InventoryItems.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.InventoryItems.RemoveRange(await LoadSomeAsync(ids));

        public Task<decimal> FindTotalAmountInStockAsync(Article article)
            => _dbContext.InventoryItems
                .Where(e => e.Article == article &&
                            e.Shop != null)
                .SumAsync(e => e.Amount);

        public Task<decimal> FindSpareInventoryAsync(Article article)
            => _dbContext.InventoryItems
                .Where(e => e.Article == article &&
                            e.Shop != null &&
                            e.Order == null)
                .SumAsync(e => e.Amount);

        public async Task<decimal> FindTotalAmountInOrdersAsync(Article article)
            => await _dbContext.InventoryItems.Where(e => e.Article == article && e.Order != null)
                .SumAsync(e => e.Amount);
    }
}