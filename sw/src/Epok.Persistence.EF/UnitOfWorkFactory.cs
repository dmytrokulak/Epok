using Epok.Core.Persistence;
using System;

namespace Epok.Persistence.EF
{
    public class UnitOfWorkFactory<T> : IUnitOfWorkFactory<IUnitOfWork> where T : IUnitOfWork
    {
        private readonly DomainContext _context;
        private readonly IEntityIdentifiersKeeper _idsKeeper;

        public UnitOfWorkFactory(DomainContext context, IEntityIdentifiersKeeper idsKeeper)
        {
            _context = context;
            _idsKeeper = idsKeeper;
        }

        public IUnitOfWork Transact() 
            => (T) Activator.CreateInstance(typeof(T), _context, _idsKeeper);
    }
}
