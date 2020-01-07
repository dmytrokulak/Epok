using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Users.Entities
{
    /// <summary>
    /// CQRS resource being a command, query or event handler.
    /// </summary>
    public class CqrsResource : EntityBase
    {
        public CqrsResource(Guid id, string name) : base(id, name)
        {
        }

        public string KeyEntityName { get; set; }
    }
}
