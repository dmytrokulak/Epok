using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Shops;
using Epok.Domain.Shops.Commands;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Queries;
using Epok.Presentation.Model;
using Epok.Presentation.Model.Shops;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage shop category entities.
    /// </summary>
    [Route("api/shopcategories")]
    [ApiController]
    public class ShopCategoriesController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage shop category entities.
        /// </summary>
        public ShopCategoriesController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of shop categories.
        /// </summary>
        /// <param name="nameLike">ShopCategory name.</param>
        /// <param name="typeExact">ShopType.</param>
        /// <param name="take">Number of entities to return in response.</param>
        /// <param name="skip">Number of entities to skip when returning in response.</param>
        /// <param name="orderBy">Property name. Sorted by "name" by default</param>
        /// <param name="orderMode">Either "asc" or "desc", "asc" by default.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ShopCategory>> GetAsync([FromQuery] string nameLike,
                [FromQuery] ShopType? typeExact, int? take, int? skip, string orderBy, string orderMode)
            //ToDo:4 optionally include archived?
        {
            var query = new ShopCategoriesQuery
            {
                Take = take,
                Skip = skip,
                OrderBy = orderBy,
                OrderMode = orderMode,
                FilterNameLike = nameLike,
                FilterShopTypeExact = typeExact,
            };
            //ToDo:2 query.AsLazy(); ??
            return await _queryInvoker.Execute<ShopCategoriesQuery, ShopCategory>(query);
        }

        /// <summary>
        /// Returns a shop category by id.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ShopCategory> GetAsync(Guid id)
        {
            var query = new ShopCategoriesQuery {FilterIds = id.Collect()};
            return (await _queryInvoker.Execute<ShopCategoriesQuery, ShopCategory>(query)).SingleOrDefault();
        }

        /// <summary>
        /// Creates a new shop category in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync([FromBody] CreateShopCategoryModel model)
        {
            var command = _mapper.Map<CreateShopCategory>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Sets given shop as default for the category.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <param name="subId">Shop id.</param>
        /// <returns></returns>
        [HttpPut("{id}/shops/{subId}/default")]
        public async Task PutShopAsDefaultAsync(Guid id, Guid subId)
        {
            var command = new SetDefaultShopForCategory()
            {
                ShopId = subId,
                ShopCategoryId = id,
                InitiatorId = Guid.NewGuid()
            };
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Allows article to be stored in shops of this category.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <param name="subId">Article id</param>
        /// <returns></returns>
        [HttpPut("{id}/articles/{subId}/allow")]
        public async Task PutArticleAllowedInShopCategoryAsync(Guid id, Guid subId)
        {
            await _commandInvoker.Execute(new AllowArticle {ShopCategoryId = id, ArticleId = subId, InitiatorId = Guid.NewGuid() });
        }

        /// <summary>
        /// Disallows article to be stored in shops of this category.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <param name="subId">Article id</param>
        /// <returns></returns>
        [HttpPut("{id}/articles/{subId}/disallow")]
        public async Task PutArticleDisallowedAllowedInShopCategoryAsync(Guid id, Guid subId)
        {
            await _commandInvoker.Execute(new DisallowArticle { ShopCategoryId = id, ArticleId = subId, InitiatorId = Guid.NewGuid() });
        }

        /// <summary>
        /// Removes shop category from the system.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveShopCategory {Id = id, InitiatorId = Guid.NewGuid()});
        }

        /// <summary>
        /// Returns dictionary of shop types.
        /// </summary>
        [HttpGet("types")]
        public IEnumerable<EnumModel> GetOrderTypes()
        {
            return Enum.GetValues(typeof(ShopType))
                .Cast<ShopType>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
