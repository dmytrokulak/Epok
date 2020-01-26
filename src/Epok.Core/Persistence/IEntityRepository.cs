using System;
using System.Collections.Generic;
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
        Task<T> GetAsync<T>(Guid id) where T : EntityBase;
        Task<IList<T>> GetSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase;
        Task<IList<T>> GetAllAsync<T>() where T : EntityBase;
     
        Task<T> LoadAsync<T>(Guid id) where T : EntityBase;
        Task<IList<T>> LoadSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase;
        Task<IList<T>> LoadAllAsync<T>() where T : EntityBase;

        Task AddAsync<T>(T entity) where T : EntityBase;

        Task AddRangeAsync<T>(T entities) where T : EntityBase;

        Task ArchiveAsync<T>(Guid id) where T : EntityBase;
        Task ArchiveRangeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase;
    }
}
