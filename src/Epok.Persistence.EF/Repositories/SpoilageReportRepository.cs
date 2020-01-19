using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class SpoilageReportRepository : IRepository<SpoilageReport>
    {
        private readonly DomainContext _dbContext;

        public SpoilageReportRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<SpoilageReport> LoadAsync(Guid id)
            => await _dbContext.SpoilageReports.SingleAsync(e => e.Id == id);

        public async Task<IList<SpoilageReport>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.SpoilageReports.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<SpoilageReport> GetAsync(Guid id)
            => await _dbContext.SpoilageReports
                .Include(e => e.Item)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<SpoilageReport>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.SpoilageReports
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Item)
                .ToListAsync();

        public async Task<IList<SpoilageReport>> GetAllAsync()
            => await _dbContext.SpoilageReports
                .Include(e => e.Item)
                .ToListAsync();

        public async Task AddAsync(SpoilageReport entity)
            => await _dbContext.SpoilageReports.AddAsync(entity);

        public async Task AddRangeAsync(SpoilageReport entities)
            => await _dbContext.SpoilageReports.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.SpoilageReports.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.SpoilageReports.RemoveRange(await LoadSomeAsync(ids));

    }
}
