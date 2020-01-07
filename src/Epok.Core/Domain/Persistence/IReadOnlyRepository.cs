using Epok.Core.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Epok.Core.Domain.Persistence
{ 
    /// <summary>
    /// Generic repository which resolves
    /// entity type on per query basis.
    /// </summary>
    public interface IReadOnlyRepository : IRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : IEntity;
        Task<T> LoadAsync<T>(Guid id) where T : IEntity;
    }
}
