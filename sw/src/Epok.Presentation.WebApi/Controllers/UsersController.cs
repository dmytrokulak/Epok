using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Utilities;
using Epok.Domain.Users;
using Epok.Domain.Users.Commands;
using Epok.Domain.Users.Entities;
using Epok.Domain.Users.Queries;
using Epok.Presentation.Model;
using Epok.Presentation.Model.Users;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to manage user entities.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICommandInvoker _commandInvoker;
        private readonly IQueryInvoker _queryInvoker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller to manage user entities.
        /// </summary>
        /// <param name="commandInvoker"></param>
        /// <param name="queryInvoker"></param>
        /// <param name="mapper"></param>
        public UsersController(ICommandInvoker commandInvoker, IQueryInvoker queryInvoker, IMapper mapper)
        {
            _commandInvoker = commandInvoker;
            _queryInvoker = queryInvoker;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of users: all or filtered with the parameters in query string. 
        /// </summary>
        /// <param name="nameLike">Filter by partial equality.</param>
        /// <param name="typeExact">Filter by strict equality.</param>
        /// <param name="emailLike">Filter by partial equality.</param>
        /// <param name="isShopManagerExact">Filter by strict equality.</param>
        /// <param name="take">Number of entities to return in response.</param>
        /// <param name="skip">Number of entities to skip when returning in response.</param>
        /// <param name="orderBy">Property name. Sorted by "name" by default</param>
        /// <param name="orderMode">Either "asc" or "desc", "asc" by default.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<User>> GetAsync([FromQuery] string nameLike, [FromQuery] UserType? typeExact,
            [FromQuery] string emailLike, [FromQuery] bool? isShopManagerExact, int? take, int? skip, 
            string orderBy, string orderMode) //ToDo:4 optionally include archived?
        {
            var query = new UsersQuery
            {
                Take = take,
                Skip = skip,
                OrderBy = orderBy,
                OrderMode = orderMode,
                FilterNameLike = nameLike,
                FilterUserTypeExact = typeExact,
                FilterEmailLike = emailLike,
                FilterIsShopManagerExact = isShopManagerExact
            };
            //ToDo:2 query.AsLazy(); ??
            return await _queryInvoker.Execute<UsersQuery, User>(query);
        }

        /// <summary>
        /// Returns a single User by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<User> GetAsync(Guid id)
        {
            var query = new UsersQuery {FilterIds = id.Collect()};
            return (await _queryInvoker.Execute<UsersQuery, User>(query)).SingleOrDefault();
        }

        /// <summary>
        /// Creates a new User in the system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostAsync([FromBody] UserModel model)
        {
            var command = _mapper.Map<CreateUser>(model);
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Modifies User by User id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task PutUserAsync(Guid id, [FromBody] UserModel model)
        {
            var command = _mapper.Map<ChangeUserData>(model);
            command.Id = id;
            //ToDo:3  command.InitiatorId = User.Identity.Id;
            command.InitiatorId = Guid.NewGuid();
            await _commandInvoker.Execute(command);
        }

        /// <summary>
        /// Removes User from the system.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _commandInvoker.Execute(new ArchiveUser {Id = id, InitiatorId = Guid.NewGuid()});
        }


        /// <summary>
        /// Returns dictionary of user types.
        /// </summary>
        [HttpGet("types")]
        public IEnumerable<EnumModel> GetUserTypes()
        {
            return Enum.GetValues(typeof(UserType))
                .Cast<UserType>()
                .Select(t => EnumModel.New((int)t, t.ToString()));
        }
    }
}
