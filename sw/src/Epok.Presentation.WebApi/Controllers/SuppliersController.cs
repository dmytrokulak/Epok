using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Commands;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Suppliers;
using Epok.Domain.Suppliers.Commands;
using Epok.Domain.Suppliers.Entities;
using Epok.Domain.Suppliers.Queries;
using Epok.Presentation.Model;
using Epok.Presentation.Model.Customers;
using Epok.Presentation.Model.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage supplier entities.
    /// </summary>
    [Route("api/suppliers")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage Supplier entities.
        /// </summary>
        /// <param name="commandInvoker"></param>
        /// <param name="queryInvoker"></param>
        /// <param name="mapper"></param>
        public SuppliersController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of Suppliers: all or filtered with the parameters in query string. 
        /// </summary>
        /// <param name="nameLike">Filter by partial equality.</param>
        /// <param name="articleExact">Filter by strict equality.</param>
        /// <param name="countryExact">Filter by strict equality.</param>
        /// <param name="provinceExact">Filter by strict equality.</param>
        /// <param name="cityExact">Filter by strict equality.</param>
        /// <param name="take">Number of entities to return in response.</param>
        /// <param name="skip">Number of entities to skip when returning in response.</param>
        /// <param name="orderBy">Property name. Sorted by "name" by default</param>
        /// <param name="orderMode">Either "asc" or "desc", "asc" by default.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SupplierModel>> GetAsync([FromQuery] string nameLike,
            [FromQuery] Guid? articleExact, [FromQuery] string countryExact,
            [FromQuery] string provinceExact, [FromQuery] string cityExact, int? take, int? skip, 
            string orderBy, string orderMode) //ToDo:4 optionally include archived?
        {
            var query = new SuppliersQuery
            {
                Take = take,
                Skip = skip,
                OrderBy = orderBy,
                OrderMode = orderMode,
                FilterNameLike = nameLike,
                FilterArticleIdExact = articleExact,
                FilterCountryExact = countryExact,
                FilterProvinceExact = provinceExact,
                FilterCityExact = cityExact
            };
            //ToDo:2 query.AsLazy(); ??
            var suppliers = await _queryInvoker.Execute<SuppliersQuery, Supplier>(query);
            return _mapper.Map<IEnumerable<SupplierModel>>(suppliers);
        }

        /// <summary>
        /// Returns a single supplier by id.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<SupplierModel> GetAsync(Guid id)
        {
            var query = new SuppliersQuery { FilterIds = id.Collect() };
            var supplier = (await _queryInvoker.Execute<SuppliersQuery, Supplier>(query)).SingleOrDefault();
            return _mapper.Map<SupplierModel>(supplier);
        }

        /// <summary>
        /// Returns a collection of material requests for this supplier.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <returns></returns>
        [HttpGet("{id}/requests")]
        public async Task<IEnumerable<MaterialRequestModel>> GetMaterialRequestsAsync(Guid id)
        {
            var query = new MaterialRequestsQuery {FilterSupplierIdExact = id};
            var materialRequests = await _queryInvoker.Execute<MaterialRequestsQuery, MaterialRequest>(query);
            return _mapper.Map<IEnumerable<MaterialRequestModel>>(materialRequests);
        }


        /// <summary>
        /// Returns collection of supplier's contacts.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <returns></returns>
        [HttpGet("{id}/contacts")]
        public async Task<IEnumerable<Contact>> GetContactsAsync(Guid id)
        {
            var query = new SuppliersQuery { FilterIds = id.Collect() };
            return (await _queryInvoker.Execute<SuppliersQuery, Supplier>(query)).SingleOrDefault()?.Contacts;
        }


        /// <summary>
        /// Modifies Supplier's shipping address by Supplier id.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/address")]
        public async Task PutSupplierAddressAsync(Guid id, [FromBody] ChangeSupplierAddressModel model)
        {
            var command = _mapper.Map<ChangeSupplierAddress>(model);
            command.Id = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Adds a new contact to Supplier's collection of contacts.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/contacts")]
        public async Task PostSupplierContactAsync(Guid id, [FromBody] ContactModel model)
        {
            var command = _mapper.Map<RegisterContact>(model);
            command.CompanyId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies Supplier's contact by Supplier id and contact id.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Contact id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/contacts/{subId}")]
        public async Task PutSupplierContactAsync(Guid id, Guid subId, [FromBody] ContactModel model)
        {
            var command = _mapper.Map<ChangeSupplierContact>(model);
            command.Id = subId;
            command.SupplierId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Sets specified contact as primary contact for the Supplier.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Contact id.</param>
        /// <returns></returns>
        [HttpPut("{id}/contacts/{subId}/primary")]
        public async Task PutSupplierContactAsPrimaryAsync(Guid id, Guid subId)
        {
            var command = new SetSupplierPrimaryContact
            {
                NewPrimaryContactId = subId,
                SupplierId = id,
                InitiatorId = Guid.NewGuid()
            };
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Creates a new material request for this supplier.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <returns></returns>
        [HttpPost("{id}/requests")]
        public async Task PostMaterialRequestsAsync(Guid id, [FromBody] CreateMaterialRequestModel model)
        {
            var command = _mapper.Map<CreateMaterialRequest>(model);
            command.SupplierId = id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Receive materials from the given request. 
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Material request id.</param>
        /// <returns></returns>
        [HttpPut("{id}/requests/{subId}/receive")]
        public async Task PutMaterialRequestReceiveAsync(Guid id, Guid subId)
        {
            var command = new ReceiveMaterials()
            {
                MaterialRequestId = subId,
                InitiatorId = Guid.NewGuid()
            };
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        ///  Add article as suppliable by this supplier.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Article id.</param>
        /// <returns></returns>
        [HttpPut("{id}/suppliable/{subId}/add")]
        public async Task PutMaterialAsSuppliableAsync(Guid id, Guid subId)
        {
            var command = new AddArticleToSuppliable
            {
                SupplierId = id,
                ArticleId = subId,
                InitiatorId = Guid.NewGuid()
            };

            await _commandInvoker.Execute(command);
        }

        /// <summary>
        ///  Remove article from the collection of articles suppliable by this supplier.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Article id.</param>
        /// <returns></returns>
        [HttpPut("{id}/suppliable/{subId}/remove")]
        public async Task PutMaterialAsNonSuppliableAsync(Guid id, Guid subId)
        {
            var command = new RemoveArticleFromSuppliable
            {
                SupplierId = id,
                ArticleId = subId,
                InitiatorId = Guid.NewGuid()
            };

            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Creates a new supplier in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync([FromBody] RegisterSupplierModel model)
        {
            var command = _mapper.Map<RegisterSupplier>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Removes supplier from the system.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveSupplier { Id = id, InitiatorId = Guid.NewGuid() });
        }

        /// <summary>
        /// Removes contact from the system.
        /// </summary>
        /// <param name="id">Supplier id.</param>
        /// <param name="subId">Contact id</param>
        /// <returns></returns>
        [HttpDelete("{id}/contact/{subId}")]
        public async Task DeleteContactAsync(Guid id, Guid subId)
        {
            await _commandInvoker.Execute(new ArchiveContact { Id = subId, CompanyId = id, InitiatorId = Guid.NewGuid() });
        }

        /// <summary>
        /// Returns dictionary of material request statuses.
        /// </summary>
        [HttpGet("requests/statuses")]
        public IEnumerable<EnumModel> GetMaterialRequestStatuses()
        {
            return Enum.GetValues(typeof(MaterialRequestStatus))
                .Cast<MaterialRequestStatus>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
