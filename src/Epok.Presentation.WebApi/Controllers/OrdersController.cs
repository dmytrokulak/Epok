using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Orders;
using Epok.Domain.Orders.Commands;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Orders.Queries;
using Epok.Presentation.Model;
using Epok.Presentation.Model.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage order entities.
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage order entities.
        /// </summary>

        public OrdersController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }


        /// <summary>
        /// Returns a collection of Orders: all or filtered with the parameters in query string. 
        /// </summary>
        /// <param name="nameLike">Filter by partial equality.</param>
        /// <param name="statusExact">Filter by exact equality.</param>
        /// <param name="typeExact">Filter by exact equality.</param>
        /// <param name="customer">Filter by exact equality.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Order>> GetAsync([FromQuery] string nameLike,
                [FromQuery] OrderType? typeExact, [FromQuery] OrderStatus? statusExact, [FromQuery] Guid? customer)
            //ToDo:4 optionally include archived?
        {
            var query = new OrdersQuery
            {
                FilterNameLike = nameLike,
                FilterTypeExact = typeExact,
                FilterStatusExact = statusExact,
                FilterCustomerExact = customer
            };
            //ToDo:2 query.AsLazy(); ??
            return await _queryInvoker.Execute<OrdersQuery, Order>(query);
        }


        /// <summary>
        /// Returns a single order by id.
        /// </summary>
        /// <param name="id">Order id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Order> GetAsync(Guid id)
        {
            var query = new OrdersQuery {FilterIds = id.Collect()};
            return (await _queryInvoker.Execute<OrdersQuery, Order>(query)).SingleOrDefault();
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public async Task PostAsync([FromBody] CreateOrderModel model)
        {
            var command = _mapper.Map<CreateOrder>(model);
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Creates manufacturing (internal) suborders for
        /// this customer (external) order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/suborders")]
        public async Task PostCreateSubOrdersAsync(Guid id)
        {
            var command = new CreateSubOrders()
            {
                OrderId = id,
                InitiatorId = Guid.NewGuid()
            };
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Signals that production process should start
        /// for this order and its suborders.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/enact")]
        public async Task PutOrderEnactAsync(Guid id)
        {
            var command = new EnactOrder
            {
                OrderId = id,
                InitiatorId = Guid.NewGuid()
            };
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Ships orders.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}/ship")]
        public async Task PutOrderShippedAsync(Guid id)
        {
            var command = new ShipOrder
            {
                Id = id,
                InitiatorId = Guid.NewGuid()
            };
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Returns dictionary of order types.
        /// </summary>
        [HttpGet("types")]
        public IEnumerable<EnumModel> GetOrderTypes()
        {
            return Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }

        /// <summary>
        /// Returns dictionary of order statuses.
        /// </summary>
        [HttpGet("statuses")]
        public IEnumerable<EnumModel> GetOrderStatuses()
        {
            return Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
