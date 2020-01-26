using Epok.Core.Persistence;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly DomainContext _dbContext;
        private readonly IEntityRepository _entityRepo;

        public ShopRepository(DomainContext dbContext, IEntityRepository entityRepo)
        {
            _dbContext = dbContext;
            _entityRepo = entityRepo;
        }

        public async Task<Shop> GetEntryPoint()
            => await _dbContext.Shops.SingleAsync(s => s.IsEntryPoint);

        public async Task<Shop> GetExitPoint()
            => await _dbContext.Shops.SingleAsync(s => s.IsExitPoint);


        #region Entities Repository Delegation

        public async Task<Shop> LoadAsync(Guid id)
            => await _entityRepo.LoadAsync<Shop>(id);

        public async Task<IList<Shop>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.LoadSomeAsync<Shop>(ids);

        public async Task<IList<Shop>> LoadAllAsync()
            => await _entityRepo.LoadAllAsync<Shop>();

        public async Task<Shop> GetAsync(Guid id)
            => await _entityRepo.GetAsync<Shop>(id);

        public async Task<IList<Shop>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.GetSomeAsync<Shop>(ids);

        public async Task<IList<Shop>> GetAllAsync()
            => await _entityRepo.GetAllAsync<Shop>();

        public async Task AddAsync(Shop entity)
            => await _entityRepo.AddAsync(entity);

        public async Task AddRangeAsync(Shop entities)
            => await _entityRepo.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => await _entityRepo.ArchiveAsync<Shop>(id);

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.ArchiveRangeAsync<Shop>(ids);

        #endregion
    }
}
