using Epok.Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly DomainContext _dbContext;

        public CustomerRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Customer> LoadAsync(Guid id)
            => await _dbContext.Customers.SingleAsync(e => e.Id == id);

        public async Task<IList<Customer>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Customers.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Customer> GetAsync(Guid id)
            => await _dbContext.Customers
                .Include(e => e.Contacts)
                .Include(e => e.Orders)
                .Include(e => e.ShippingAddress)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Customer>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Customers
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Contacts)
                .Include(e => e.Orders)
                .Include(e => e.ShippingAddress)
                .ToListAsync();

        public async Task<IList<Customer>> GetAllAsync()
            => await _dbContext.Customers
                .Include(e => e.Contacts)
                .Include(e => e.Orders)
                .Include(e => e.ShippingAddress)
                .ToListAsync();

        public async Task AddAsync(Customer entity)
            => await _dbContext.Customers.AddAsync(entity);

        public async Task AddRangeAsync(Customer entities)
            => await _dbContext.Customers.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Customers.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Customers.RemoveRange(await LoadSomeAsync(ids));
    }
}