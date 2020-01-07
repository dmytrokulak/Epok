using Epok.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Users.Entities
{
    public class Permission : EntityBase
    {
        public Permission(Guid id, string name) : base(id, name)
        {
        }

        public User User { get; set; }
        public CqrsResource Handler { get; set; }

        /// <summary>
        /// Filter to restrict by a key entity id
        /// </summary>
        public IEnumerable<Guid> Restriction { get; set; }
    }
}
