using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class BillOfMaterialRepository : IRepository<BillOfMaterial>
    {
        private readonly DomainContext _dbContext;

        public BillOfMaterialRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<BillOfMaterial> LoadAsync(Guid id)
            => await _dbContext.BillsOfMaterial.SingleAsync(e => e.Id == id);

        public async Task<IList<BillOfMaterial>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.BillsOfMaterial.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<BillOfMaterial> GetAsync(Guid id)
            => await _dbContext.BillsOfMaterial
                .Include(e => e.Article)
                .Include(e => e.Input).ThenInclude(e => e.Article)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<BillOfMaterial>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.BillsOfMaterial
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Article)
                .Include(e => e.Input).ThenInclude(e => e.Article)
                .ToListAsync();

        public async Task<IList<BillOfMaterial>> GetAllAsync()
            => await _dbContext.BillsOfMaterial
                .Include(e => e.Article)
                .Include(e => e.Input).ThenInclude(e => e.Article)
                .ToListAsync();

        public async Task AddAsync(BillOfMaterial entity)
            => await _dbContext.BillsOfMaterial.AddAsync(entity);

        public async Task AddRangeAsync(BillOfMaterial entity)
            => await _dbContext.BillsOfMaterial.AddRangeAsync(entity);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.BillsOfMaterial.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.BillsOfMaterial.RemoveRange(await LoadSomeAsync(ids));
    }
}