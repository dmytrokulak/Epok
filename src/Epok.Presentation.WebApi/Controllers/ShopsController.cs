using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Shops.Commands;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Shops.Queries;
using Epok.Presentation.WebApi.Models.Shops;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage shop entities.
    /// </summary>
    [Route("api/shops")]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage shop entities.
        /// </summary>
        public ShopsController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of shops.
        /// </summary>
        /// <param name="nameLike"> Shop name.</param>
        /// <param name="categoryExact">ShopCategory id.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Shop>> GetAsync([FromQuery] string nameLike, [FromQuery] Guid? categoryExact)
            //ToDo:4 optionally include archived?
        {
            var query = new ShopsQuery
            {
                FilterNameLike = nameLike,
                FilterShopCategoryExact = categoryExact,
            };
            //ToDo:2 query.AsLazy(); ??
            var shops = await _queryInvoker.Execute<ShopsQuery, Shop>(query);
            foreach (var shop in shops)
            {
                shop.ShopCategory.Shops = null;
                shop.Manager.Shop = null;
            }
            return shops;
        }

        /// <summary>
        /// Returns a shop by id.
        /// </summary>
        /// <param name="id">Shop category id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Shop> GetAsync(Guid id)
        {
            var query = new ShopsQuery {FilterIds = id.Collect()};
            var shop = (await _queryInvoker.Execute<ShopsQuery, Shop>(query)).SingleOrDefault();
            if (shop == null)
                return null;
            //ToDo:2 limit depth or use lazy loading
            shop.ShopCategory.Shops = null;
            shop.Manager.Shop = null;
            return shop;
        }

        /// <summary>
        /// Creates a new shop in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync([FromBody] CreateShopModel model)
        {
            var command = _mapper.Map<CreateShop>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Assigns new manager to the shop.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subId"></param>
        [HttpPut("{id}/manager/{subId}")]
        public async void PutManagerAsync(Guid id, Guid subId)
        {
            await _commandInvoker
                .Execute(new ChangeShopManager {ShopId = id, NewManagerId = subId, InitiatorId = Guid.NewGuid()});
        }

        /// <summary>
        /// Removes shop from the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveShop {Id = id, InitiatorId = Guid.NewGuid()});
        }
    }
}
