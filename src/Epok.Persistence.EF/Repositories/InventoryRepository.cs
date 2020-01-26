using Epok.Core.Persistence;
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
        private readonly IEntityRepository _entityRepo;

        public InventoryRepository(DomainContext dbContext, IEntityRepository entityRepo)
        {
            _dbContext = dbContext;
            _entityRepo = entityRepo;
        }

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


        #region Entities Repository Delegation

        public async Task<InventoryItem> LoadAsync(Guid id)
            => await _entityRepo.LoadAsync<InventoryItem>(id);

        public async Task<IList<InventoryItem>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.LoadSomeAsync<InventoryItem>(ids);

        public async Task<IList<InventoryItem>> LoadAllAsync()
            => await _entityRepo.LoadAllAsync<InventoryItem>();

        public async Task<InventoryItem> GetAsync(Guid id)
            => await _entityRepo.GetAsync<InventoryItem>(id);

        public async Task<IList<InventoryItem>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.GetSomeAsync<InventoryItem>(ids);

        public async Task<IList<InventoryItem>> GetAllAsync()
            => await _entityRepo.GetAllAsync<InventoryItem>();

        public async Task AddAsync(InventoryItem entity)
            => await _entityRepo.AddAsync(entity);

        public async Task AddSomeAsync(InventoryItem entities)
            => await _entityRepo.AddSomeAsync(entities);

        public async Task RemoveAsync(InventoryItem entity)
            => await _entityRepo.RemoveAsync(entity);

        public async Task RemoveSomeAsync(IEnumerable<InventoryItem> entities)
            => await _entityRepo.RemoveSomeAsync(entities);

        #endregion
    }
}