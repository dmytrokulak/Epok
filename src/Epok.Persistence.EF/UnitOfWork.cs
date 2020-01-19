using Epok.Core.Persistence;

namespace Epok.Persistence.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DomainContext _context;

        public UnitOfWork(DomainContext context)
        {
            _context = context;
            _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _context.SaveChanges();
            _context.Database.CommitTransaction();
        }
    }
}
