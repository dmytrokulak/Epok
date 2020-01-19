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
    public interface IReadOnlyRepository : IRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : EntityBase;
        Task<IList<T>> GetSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase;
        Task<T> LoadAsync<T>(Guid id) where T : EntityBase;
        Task<IList<T>> LoadSomeAsync<T>(IEnumerable<Guid> ids) where T : EntityBase;
    }
}
