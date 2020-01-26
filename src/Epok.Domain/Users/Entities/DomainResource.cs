using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Users.Entities
{
    /// <summary>
    /// Domain resource is exposed through a command, query or event handler.
    /// </summary>
    public class DomainResource : EntityBase
    {
        /// <summary>
        /// ORM constructor.
        /// </summary>
        public DomainResource()
        {
        }
        public DomainResource(Guid id, string name) : base(id, name)
        {
        }

        public virtual string KeyEntityName { get; set; }
    }
}
