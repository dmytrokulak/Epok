using Epok.Core.Persistence;

namespace Epok.Persistence.EF
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory<UnitOfWork>
    {
        private readonly DomainContext _context;

        public UnitOfWorkFactory(DomainContext context)
        {
            _context = context;
        }

        public UnitOfWork New()
        {
            return new UnitOfWork(_context);
        }
    }
}
