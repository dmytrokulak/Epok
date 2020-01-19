using Epok.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class SupplierRepository : IRepository<Supplier>
    {
        private readonly DomainContext _dbContext;

        public SupplierRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Supplier> LoadAsync(Guid id)
            => await _dbContext.Suppliers.SingleAsync(e => e.Id == id);

        public async Task<IList<Supplier>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Suppliers.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Supplier> GetAsync(Guid id)
            => await _dbContext.Suppliers
                .Include(e => e.Contacts)
                .Include(e => e.MaterialRequests)
                .Include(e => e.ShippingAddress)
                .Include(e => e.SuppliableArticles)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Supplier>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Suppliers
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Contacts)
                .Include(e => e.MaterialRequests)
                .Include(e => e.ShippingAddress)
                .Include(e => e.SuppliableArticles).ToListAsync();

        public async Task<IList<Supplier>> GetAllAsync()
            => await _dbContext.Suppliers
                .Include(e => e.Contacts)
                .Include(e => e.MaterialRequests)
                .Include(e => e.ShippingAddress)
                .Include(e => e.SuppliableArticles).ToListAsync();

        public async Task AddAsync(Supplier entity)
            => await _dbContext.Suppliers.AddAsync(entity);

        public async Task AddRangeAsync(Supplier entities)
            => await _dbContext.Suppliers.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Suppliers.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Suppliers.RemoveRange(await LoadSomeAsync(ids));

    }
}
