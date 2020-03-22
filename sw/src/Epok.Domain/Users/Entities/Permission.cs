using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Users.Entities
{
    public class Permission : EntityBase
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public Permission()
        {
        }
        public Permission(Guid id, string name) : base(id, name)
        {
        }

        public virtual User User { get; set; }
        public virtual DomainResource Resource { get; set; }

        // Filter to restrict by a key entity id
        //ToDo:4 public virtual IEnumerable<Guid> Restriction { get; set; }
    }
}
