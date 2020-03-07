using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Queries;
using Epok.Core.Persistence;
using Epok.Domain.Inventory.Entities;

namespace Epok.Domain.Inventory.Queries.Handlers
{
    public class ArticlesQueryHandler : IQueryHandler<ArticlesQuery, Article>
    {
        private readonly IEntityRepository _repository;

        public ArticlesQueryHandler(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<Article>> HandleAsync(ArticlesQuery query)
        {
            Expression<Func<Article, bool>> predicate = article =>
                (query.FilterNameLike == null || article.Name.Contains(query.FilterNameLike)) &&
                (query.FilterArticleTypeExact == null || article.ArticleType == query.FilterArticleTypeExact) &&
                (query.FilterUomExact == null || article.UoM.Id == query.FilterUomExact) &&
                (query.FilterCodeLike == null || article.Code == query.FilterCodeLike);

            if (query.Lazy)
                return await _repository.LoadSomeAsync(query.FilterIds, predicate);
            return await _repository.GetSomeAsync(query.FilterIds, predicate);
        }
    }
}
