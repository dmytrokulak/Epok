using Epok.Core.Domain.Persistence;
using Epok.Domain.Users.Entities;
using System.Threading.Tasks;

namespace Epok.Domain.Users.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission> Find(User user, CqrsResource resource);
    }
}
