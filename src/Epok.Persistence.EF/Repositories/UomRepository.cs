using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;

namespace Epok.Persistence.EF.Repositories
{
    public class UomRepository : IRepository<Uom>
    {
        private readonly DomainContext _dbContext;

        public UomRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Uom> LoadAsync(Guid id)
            => await _dbContext.Uoms.SingleAsync(e => e.Id == id);

        public async Task<IList<Uom>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Uoms.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Uom> GetAsync(Guid id)
            => await _dbContext.Uoms.SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Uom>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Uoms
                .Where(e => ids.Contains(e.Id))
                .ToListAsync();

        public async Task<IList<Uom>> GetAllAsync()
            => await _dbContext.Uoms.ToListAsync();

        public async Task AddAsync(Uom entity)
            => await _dbContext.Uoms.AddAsync(entity);

        public async Task AddRangeAsync(Uom entities)
            => await _dbContext.Uoms.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Uoms.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Uoms.RemoveRange(await LoadSomeAsync(ids));
    }
}
