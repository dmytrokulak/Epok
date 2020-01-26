using Epok.Core.Persistence;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DomainContext _dbContext;
        private readonly IEntityRepository _entityRepo;

        public PermissionRepository(DomainContext dbContext, IEntityRepository entityRepo)
        {
            _dbContext = dbContext;
            _entityRepo = entityRepo;
        }

        public async Task<Permission> Find(User user, DomainResource resource)
            => await _dbContext.Permissions
                .SingleOrDefaultAsync(p => p.User == user && p.Resource == resource);



        #region Entities Repository Delegation

        public async Task<Permission> LoadAsync(Guid id)
            => await _entityRepo.LoadAsync<Permission>(id);

        public async Task<IList<Permission>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.LoadSomeAsync<Permission>(ids);

        public async Task<IList<Permission>> LoadAllAsync()
            => await _entityRepo.LoadAllAsync<Permission>();

        public async Task<Permission> GetAsync(Guid id)
            => await _entityRepo.GetAsync<Permission>(id);

        public async Task<IList<Permission>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.GetSomeAsync<Permission>(ids);

        public async Task<IList<Permission>> GetAllAsync()
            => await _entityRepo.GetAllAsync<Permission>();

        public async Task AddAsync(Permission entity)
            => await _entityRepo.AddAsync(entity);

        public async Task AddSomeAsync(Permission entities)
            => await _entityRepo.AddSomeAsync(entities);

        public async Task RemoveAsync(Permission entity)
            => await _entityRepo.RemoveAsync(entity);

        public async Task RemoveSomeAsync(IEnumerable<Permission> entities)
            => await _entityRepo.RemoveSomeAsync(entities);

        #endregion
    }
}
