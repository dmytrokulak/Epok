using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epok.Core.Domain.Entities;

namespace Epok.Core.Persistence
{ 
    /// <summary>
    /// Generic repository which resolves
    /// entity type on per query basis.
    /// </summary>
    public interface IEntityRepository : IRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : EntityBase; //ToDo:4 change to IEntity?
        Task<IList<T>> GetSomeAsync<T>(IEnumerable<Guid> ids = null, Expression<Func<T, bool>> predicate = null) where T : EntityBase;
        Task<IList<T>> GetAllAsync<T>() where T : EntityBase;
     
        Task<T> LoadAsync<T>(Guid id) where T : EntityBase;
        Task<IList<T>> LoadSomeAsync<T>(IEnumerable<Guid> ids = null, Expression<Func<T, bool>> predicate = null) where T : EntityBase;
        Task<IList<T>> LoadAllAsync<T>() where T : EntityBase;

        Task AddAsync<T>(T entity) where T : EntityBase;

        Task AddSomeAsync<T>(T entities) where T : EntityBase;

        Task RemoveAsync<T>(T entity) where T : EntityBase;
        Task RemoveSomeAsync<T>(IEnumerable<T> entities) where T : EntityBase;
    }
}
