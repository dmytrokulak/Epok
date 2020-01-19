using Epok.Domain.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Persistence.EF.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly DomainContext _dbContext;

        public OrderRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Order> LoadAsync(Guid id)
            => await _dbContext.Orders.SingleAsync(e => e.Id == id);

        public async Task<IList<Order>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Orders.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Order> GetAsync(Guid id)
            => await _dbContext.Orders
                .Include(e => e.Customer)
                .Include(e => e.ItemsOrdered)
                .Include(e => e.ItemsProduced)
                .Include(e => e.ParentOrder)
                .Include(e => e.ReferenceOrder)
                .Include(e => e.Shop)
                .Include(e => e.SubOrders)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Order>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Orders
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.Customer)
                .Include(e => e.ItemsOrdered)
                .Include(e => e.ItemsProduced)
                .Include(e => e.ParentOrder)
                .Include(e => e.ReferenceOrder)
                .Include(e => e.Shop)
                .Include(e => e.SubOrders)
                .ToListAsync();

        public async Task<IList<Order>> GetAllAsync()
            => await _dbContext.Orders
                .Include(e => e.Customer)
                .Include(e => e.ItemsOrdered)
                .Include(e => e.ItemsProduced)
                .Include(e => e.ParentOrder)
                .Include(e => e.ReferenceOrder)
                .Include(e => e.Shop)
                .Include(e => e.SubOrders)
                .ToListAsync();

        public async Task AddAsync(Order entity)
            => await _dbContext.Orders.AddAsync(entity);

        public async Task AddRangeAsync(Order entities)
            => await _dbContext.Orders.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Orders.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Orders.RemoveRange(await LoadSomeAsync(ids));
    }
}