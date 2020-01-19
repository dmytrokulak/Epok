using Epok.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class MaterialRequestRepository : IRepository<MaterialRequest>
    {
        private readonly DomainContext _dbContext;

        public MaterialRequestRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<MaterialRequest> LoadAsync(Guid id)
            => await _dbContext.MaterialRequests.SingleAsync(e => e.Id == id);

        public async Task<IList<MaterialRequest>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.MaterialRequests.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<MaterialRequest> GetAsync(Guid id)
            => await _dbContext.MaterialRequests
                .Include(e => e.Supplier)
                .Include(e => e.ItemsRequested)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<MaterialRequest>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.MaterialRequests
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Supplier)
                .Include(e => e.ItemsRequested)
                .ToListAsync();

        public async Task<IList<MaterialRequest>> GetAllAsync()
            => await _dbContext.MaterialRequests
                .Include(e => e.Supplier)
                .Include(e => e.ItemsRequested)
                .ToListAsync();

        public async Task AddAsync(MaterialRequest entity)
            => await _dbContext.MaterialRequests.AddAsync(entity);

        public async Task AddRangeAsync(MaterialRequest entities)
            => await _dbContext.MaterialRequests.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.MaterialRequests.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.MaterialRequests.RemoveRange(await LoadSomeAsync(ids));
    }
}