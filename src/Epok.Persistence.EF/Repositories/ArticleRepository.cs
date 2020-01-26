using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DomainContext _dbContext;
        private readonly IEntityRepository _entityRepo;

        public ArticleRepository(DomainContext dbContext, IEntityRepository entityRepo)
        {
            _dbContext = dbContext;
            _entityRepo = entityRepo;
        }

        public Task<SpoiledArticle> FindSpoiledCounterpartAsync(Article article, bool fixable) =>
            _dbContext.SpoiledArticles.SingleOrDefaultAsync(e => e.Article == article && e.Fixable == fixable);

        #region Entities Repository Delegation

        public async Task<Article> LoadAsync(Guid id)
            => await _entityRepo.LoadAsync<Article>(id);

        public async Task<IList<Article>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.LoadSomeAsync<Article>(ids);

        public async Task<IList<Article>> LoadAllAsync()
            => await _entityRepo.LoadAllAsync<Article>();

        public async Task<Article> GetAsync(Guid id)
            => await _entityRepo.GetAsync<Article>(id);

        public async Task<IList<Article>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _entityRepo.GetSomeAsync<Article>(ids);

        public async Task<IList<Article>> GetAllAsync()
            => await _entityRepo.GetAllAsync<Article>();

        public async Task AddAsync(Article entity)
            => await _entityRepo.AddAsync(entity);

        public async Task AddSomeAsync(Article entities)
            => await _entityRepo.AddSomeAsync(entities);

        public async Task RemoveAsync(Article entity)
            => await _entityRepo.RemoveAsync(entity);

        public async Task RemoveSomeAsync(IEnumerable<Article> entities)
            => await _entityRepo.RemoveSomeAsync(entities);

        #endregion
    }
}