using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Commands;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Queries;
using Epok.Presentation.Model;
using Epok.Presentation.Model.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage inventory entities: articles, bill of materials and spoilage.
    /// </summary>
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage inventory entities: articles, bill of materials and spoilage.
        /// </summary>
        /// <param name="commandInvoker"></param>
        /// <param name="queryInvoker"></param>
        /// <param name="mapper"></param>
        public InventoryController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of articles.
        /// </summary>
        /// <param name="nameLike"></param>
        /// <param name="typeExact"></param>
        /// <param name="uomExact"></param>
        /// <param name="codeLike"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ArticleModel>> GetAsync([FromQuery] string nameLike,
            [FromQuery] ArticleType? typeExact, [FromQuery] Guid? uomExact, [FromQuery] string codeLike)
        {
            var query = new ArticlesQuery
            {
                FilterNameLike = nameLike,
                FilterArticleTypeExact = typeExact,
                FilterUomExact = uomExact,
                FilterCodeLike = codeLike,
            };
          //  query.AsLazy();
          var articles = await _queryInvoker.Execute<ArticlesQuery, Article>(query);
          return _mapper.Map<IEnumerable<ArticleModel>>(articles);
        }

        /// <summary>
        ///  Return an article by id.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ArticleModel> GetAsync(Guid id)
        {
            var query = new ArticlesQuery {FilterIds = id.Collect()};
           // query.AsLazy();
            var article = (await _queryInvoker.Execute<ArticlesQuery, Article>(query)).SingleOrDefault();
            return _mapper.Map<ArticleModel>(article);
        }

        /// <summary>
        ///  Return bills of material for this article.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <returns></returns>
        [HttpGet("{id}/boms")]
        public async Task<IEnumerable<BillOfMaterialModel>> GetBillsOfMaterialAsync(Guid id)
        {
            var query = new BillsOfMaterialQuery { FilterArticleIdExact = id };
            // query.AsLazy();
            var article = await _queryInvoker.Execute<BillsOfMaterialQuery, BillOfMaterial>(query);
            return _mapper.Map<IEnumerable<BillOfMaterialModel>>(article);
        }
        /// <summary>
        /// Creates a new article in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostArticleAsync([FromBody] RegisterArticleModel model)
        {
            var command = _mapper.Map<RegisterArticle>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Creates a new bom for the article.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/bom")]
        public async Task PostBomAsync(Guid id, [FromBody] BillOfMaterialUpsertModel model)
        {
            var command = _mapper.Map<AddBillOfMaterial>(model);
            command.ArticleId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies a given bill of material.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="subId">Bom id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/bom/{subId}")]
        public async Task PutBomAsync(Guid id, Guid subId, [FromBody] BillOfMaterialUpsertModel model)
        {
            var command = _mapper.Map<ChangeBillOfMaterial>(model);
            command.Id = subId;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Sets specified bill of material as primary for the article.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="subId">Contact id.</param>
        /// <returns></returns>
        [HttpPut("{id}/contact/{subId}/primary")]
        public async Task PutCustomerContactAsPrimaryAsync(Guid id, Guid subId)
        {
            var command = new SetPrimaryBillOfMaterial
            {
                BomId = subId,
                ArticleId = id,
                InitiatorId = Guid.NewGuid()
            };
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Produces an inventory item.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/produce")]
        public async Task PutProduceAsync(Guid id, [FromBody] ProduceInventoryModel model)
        {
            var command = _mapper.Map<ProduceInventoryItem>(model);
            command.ArticleId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Transfers an inventory item between two shops.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/transfer")]
        public async Task PutTransferAsync(Guid id, [FromBody] TransferInventoryModel model)
        {
            var command = _mapper.Map<TransferInventory>(model);
            command.ArticleId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        //TODO:4 Spoilage controller?
        /// <summary>
        /// Creates a new spoilage report.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/spoilage")]
        public async Task PostSpoilageAsync(Guid id, [FromBody] ReportSpoilageModel model)
        {
            var command = _mapper.Map<ReportSpoilage>(model);
            command.ArticleId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Removes a given article from the system.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveArticle {Id = id, InitiatorId = Guid.NewGuid()});
        }

        /// <summary>
        /// Removes a given bom from the system.
        /// </summary>
        /// <param name="id">Article id.</param>
        /// <param name="subId">Bill of material id</param>
        /// <returns></returns>
        [HttpDelete("{id}/contact/{subId}")]
        public async Task DeleteBomAsync( /*[Required]*/ Guid id, Guid subId)
        {
            await _commandInvoker.Execute(new ArchiveBillOfMaterial() {Id = subId, InitiatorId = Guid.NewGuid()});
        }

        /// <summary>
        /// Returns dictionary of article types.
        /// </summary>
        [HttpGet("types")]
        public IEnumerable<EnumModel> GetArticleTypes()
        {
            return Enum.GetValues(typeof(ArticleType))
                .Cast<ArticleType>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
