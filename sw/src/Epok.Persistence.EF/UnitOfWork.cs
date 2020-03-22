using Epok.Core.Domain.Entities;
using Epok.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Epok.Persistence.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DomainContext _context;
        private readonly IEntityIdentifiersKeeper _idsKeeper;

        public UnitOfWork(DomainContext context, IEntityIdentifiersKeeper idsKeeper)
        {
            _context = context;
            _idsKeeper = idsKeeper;
        }

        /// <remarks>
        /// All changes in a single call to SaveChanges() are applied in a transaction.
        /// If any of the changes fail, then the transaction is rolled back
        /// and none of the changes are applied to the database.
        /// This means that SaveChanges() is guaranteed to either completely succeed,
        /// or leave the database unmodified if an error occurs.
        /// https://docs.microsoft.com/en-us/ef/core/saving/transactions
        /// </remarks>
        public void Dispose()
        {
            var toRemove = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => (e.Entity.GetType(), ((IEntity) e.Entity).Id)).ToHashSet();

            var toAdd = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => (e.Entity.GetType(), ((IEntity) e.Entity).Id)).ToHashSet();

            _context.SaveChanges();

            if (toRemove.Count != 0)
                _idsKeeper.Remove(toRemove);
            if (toAdd.Count != 0)
                _idsKeeper.Add(toAdd);
        }
    }
}
