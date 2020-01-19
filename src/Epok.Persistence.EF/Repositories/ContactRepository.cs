using Epok.Domain.Contacts.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class ContactRepository : IRepository<Contact>
    {
        private readonly DomainContext _dbContext;

        public ContactRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Contact> LoadAsync(Guid id)
            => await _dbContext.Contacts.SingleAsync(e => e.Id == id);

        public async Task<IList<Contact>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Contacts.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Contact> GetAsync(Guid id)
            => await _dbContext.Contacts.SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Contact>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Contacts.Where(e => ids.Contains(e.Id))
                .ToListAsync();

        public async Task<IList<Contact>> GetAllAsync()
            => await _dbContext.Contacts.ToListAsync();

        public async Task AddAsync(Contact entity)
            => await _dbContext.Contacts.AddAsync(entity);

        public async Task AddRangeAsync(Contact entities)
            => await _dbContext.Contacts.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Contacts.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Contacts.RemoveRange(await LoadSomeAsync(ids));
    }
}