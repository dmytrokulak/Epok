using Epok.Domain.Contacts.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class AddressRepository : IRepository<Address>
    {
        private readonly DomainContext _dbContext;

        public AddressRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Address> LoadAsync(Guid id)
            => await _dbContext.Addresses.SingleAsync(e => e.Id == id);

        public async Task<IList<Address>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Addresses.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Address> GetAsync(Guid id)
            => await _dbContext.Addresses.SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Address>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Addresses.Where(e => ids.Contains(e.Id))
                .ToListAsync();

        public async Task<IList<Address>> GetAllAsync()
            => await _dbContext.Addresses.ToListAsync();

        public async Task AddAsync(Address entity)
            => await _dbContext.Addresses.AddAsync(entity);

        public async Task AddRangeAsync(Address entities)
            => await _dbContext.Addresses.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Addresses.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Addresses.RemoveRange(await LoadSomeAsync(ids));
    }
}