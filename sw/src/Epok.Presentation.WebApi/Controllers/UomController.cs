using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Queries;
using Epok.Presentation.Model;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage unit of measurements.
    /// </summary>
    [Route("api/uoms")]
    [ApiController]
    public class UomController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage unit of measurements.
        /// </summary>
        /// <param name="commandInvoker"></param>
        /// <param name="queryInvoker"></param>
        /// <param name="mapper"></param>
        public UomController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of uoms.
        /// </summary>
        /// <param name="nameLike"></param>
        /// <param name="typeExact"></param>
        /// <param name="take">Number of entities to return in response.</param>
        /// <param name="skip">Number of entities to skip when returning in response.</param>
        /// <param name="orderBy">Property name. Sorted by "name" by default</param>
        /// <param name="orderMode">Either "asc" or "desc", "asc" by default.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Uom>> GetAsync([FromQuery] string nameLike, [FromQuery] UomType? typeExact,
            int? take, int? skip, string orderBy, string orderMode)
        {
            var query = new UomQuery
            {
                Take = take,
                Skip = skip,
                OrderBy = orderBy,
                OrderMode = orderMode,
                FilterNameLike = nameLike,
                FilterTypeExact = typeExact
            };
          //  query.AsLazy();
          return await _queryInvoker.Execute<UomQuery, Uom>(query);
        }

        /// <summary>
        ///  Return an uom by id.
        /// </summary>
        /// <param name="id">Uom id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Uom> GetAsync(Guid id)
        {
            var query = new UomQuery { FilterIds = id.Collect()};
            // query.AsLazy();
            return (await _queryInvoker.Execute<UomQuery, Uom>(query)).SingleOrDefault();
        }
        
        /// <summary>
        /// Returns dictionary of unit of measurement types.
        /// </summary>
        [HttpGet("types")]
        public IEnumerable<EnumModel> GetUomTypes()
        {
            return Enum.GetValues(typeof(UomType))
                .Cast<UomType>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
