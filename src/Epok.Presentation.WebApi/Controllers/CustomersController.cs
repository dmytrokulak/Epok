using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epok.Presentation.WebApi.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Contacts.Commands;
using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers;
using Epok.Domain.Customers.Commands;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Customers.Queries;
using Epok.Domain.Orders.Entities;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage customer entities.
    /// </summary>
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage customer entities.
        /// </summary>
        /// <param name="commandInvoker"></param>
        /// <param name="queryInvoker"></param>
        /// <param name="mapper"></param>
        public CustomersController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of customers: all or filtered with the parameters in query string. 
        /// </summary>
        /// <param name="nameLike">Filter by partial equality.</param>
        /// <param name="typeExact">Filter by strict equality.</param>
        /// <param name="countryExact">Filter by strict equality.</param>
        /// <param name="provinceExact">Filter by strict equality.</param>
        /// <param name="cityExact">Filter by strict equality.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetAsync([FromQuery] string nameLike,
            [FromQuery] CustomerType? typeExact, [FromQuery] string countryExact,
            [FromQuery] string provinceExact, [FromQuery] string cityExact) //ToDo:4 optionally include archived?
        {
            var query = new CustomersQuery
            {
                FilterNameLike = nameLike,
                FilterCustomerTypeExact = typeExact,
                FilterCountryExact = countryExact,
                FilterProvinceExact = provinceExact,
                FilterCityExact = cityExact
            };
           //ToDo:2 query.AsLazy(); ??
            return await _queryInvoker.Execute<CustomersQuery, Customer>(query);
        }

        /// <summary>
        /// Returns a single customer by id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Customer> GetAsync(Guid id)
        {
            var query = new CustomersQuery {FilterIds = id.Collect()};
            return (await _queryInvoker.Execute<CustomersQuery, Customer>(query)).SingleOrDefault();
        }

        /// <summary>
        /// Returns collection of customer's orders.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns></returns>
        [HttpGet("{id}/orders")]
        public async Task<IEnumerable<Order>> GetOrdersAsync(Guid id)
        {
            var query = new CustomersQuery { FilterIds = id.Collect() };
            return (await _queryInvoker.Execute<CustomersQuery, Customer>(query)).SingleOrDefault()?.Orders;
        }

        /// <summary>
        /// Returns collection of customer's contacts.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns></returns>
        [HttpGet("{id}/contacts")]
        public async Task<IEnumerable<Contact>> GetContactsAsync(Guid id)
        {
            var query = new CustomersQuery { FilterIds = id.Collect() };
            return (await _queryInvoker.Execute<CustomersQuery, Customer>(query)).SingleOrDefault()?.Contacts;
        }

        /// <summary>
        /// Creates a new customer in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync([FromBody] RegisterCustomerModel model)
        {
            var command = _mapper.Map<RegisterCustomer>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies customer by customer id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/type")]
        public async Task PutCustomerTypeAsync(Guid id, [FromBody] ChangeCustomerTypeModel model)
        {
            var command = _mapper.Map<ChangeCustomerType>(model);
            command.Id = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies customer's shipping address by customer id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/address")]
        public async Task PutCustomerAddressAsync(Guid id, [FromBody] ChangeCustomerAddressModel model)
        {
            var command = _mapper.Map<ChangeCustomerAddress>(model);
            command.Id = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Adds a new contact to customer's collection of contacts.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/contact")]
        public async Task PostCustomerContactAsync(Guid id, [FromBody] ContactModel model)
        {
            var command = _mapper.Map<RegisterContact>(model);
            command.CompanyId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies customer's contact by customer id and contact id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="subId">Contact id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}/contact/{subId}")]
        public async Task PutCustomerContactAsync(Guid id, Guid subId, [FromBody] ContactModel model)
        {
            var command = _mapper.Map<ChangeCustomerContact>(model);
            command.Id = subId;
            command.CustomerId = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Sets specified contact as primary contact for the customer.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="subId">Contact id.</param>
        /// <returns></returns>
        [HttpPut("{id}/contact/{subId}/primary")]
        public async Task PutCustomerContactAsPrimaryAsync(Guid id, Guid subId)
        {
            var command = new SetCustomerPrimaryContact
            {
                NewPrimaryContactId = subId, CustomerId = id, InitiatorId = Guid.NewGuid()
            };
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Removes customer from the system.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveCustomer {Id = id, InitiatorId = Guid.NewGuid()});
        }

        /// <summary>
        /// Removes contact from the system.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="subId">Contact id</param>
        /// <returns></returns>
        [HttpDelete("{id}/contact/{subId}")]
        public async Task DeleteContactAsync(Guid id, Guid subId)
        {
            await _commandInvoker.Execute(new ArchiveContact { Id = subId, CompanyId = id, InitiatorId = Guid.NewGuid() });
        }
    }
}
