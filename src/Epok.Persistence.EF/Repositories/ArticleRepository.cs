using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Persistence.EF.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DomainContext _dbContext;

        public ArticleRepository(DomainContext dbContext)
            => _dbContext = dbContext;

        public async Task<Article> LoadAsync(Guid id)
            => await _dbContext.Articles.SingleAsync(e => e.Id == id);

        public async Task<IList<Article>> LoadSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Articles.Where(e => ids.Contains(e.Id)).ToListAsync();

        public async Task<Article> GetAsync(Guid id)
            => await _dbContext.Articles
                .Include(e => e.ProductionShopCategory)
                .Include(e => e.BillsOfMaterial)
                .Include(e => e.UoM)
                .SingleOrDefaultAsync(e => e.Id == id);

        public async Task<IList<Article>> GetSomeAsync(IEnumerable<Guid> ids)
            => await _dbContext.Articles
                .Where(e => ids.Contains(e.Id))
                .Include(e => e.ProductionShopCategory)
                .Include(e => e.BillsOfMaterial)
                .Include(e => e.UoM)
                .ToListAsync();

        public async Task<IList<Article>> GetAllAsync()
            => await _dbContext.Articles
                .Include(e => e.ProductionShopCategory)
                .Include(e => e.BillsOfMaterial)
                .Include(e => e.UoM)
                .ToListAsync();

        public async Task AddAsync(Article entity)
            => await _dbContext.Articles.AddAsync(entity);

        public async Task AddRangeAsync(Article entities)
            => await _dbContext.Articles.AddRangeAsync(entities);

        public async Task ArchiveAsync(Guid id)
            => _dbContext.Articles.Remove(await LoadAsync(id));

        public async Task ArchiveRangeAsync(IEnumerable<Guid> ids)
            => _dbContext.Articles.RemoveRange(await LoadSomeAsync(ids));

        public Task<SpoiledArticle> FindSpoiledCounterpartAsync(Article article, bool fixable) =>
            _dbContext.SpoiledArticles.SingleOrDefaultAsync(e => e.Article == article && e.Fixable == fixable);
    }
}