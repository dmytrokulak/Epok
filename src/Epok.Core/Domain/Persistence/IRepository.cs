using Epok.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Core.Domain.Persistence
{
    /// <summary>
    /// Marker interface for a repository.
    /// </summary>
    public interface IRepository
    {

    }

    public interface IRepository<T> : IRepository where T : IEntity
    {
        /// <summary>
        /// Creates a proxy; throws exception if object not persisted in db.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> LoadAsync(Guid id);

        Task<T> GetAsync(Guid id);
        Task<IEnumerable<T>> GetSomeAsync(IEnumerable<Guid> id);
        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        Task AddRangeAsync(T entity);

        Task ArchiveAsync(Guid id);
        Task ArchiveRangeAsync(Guid id);
    }
}
