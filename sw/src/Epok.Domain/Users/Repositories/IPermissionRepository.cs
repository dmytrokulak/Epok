using Epok.Domain.Users.Entities;
using System.Threading.Tasks;
using Epok.Core.Persistence;

namespace Epok.Domain.Users.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission> Find(User user, DomainResource resource);
    }
}
