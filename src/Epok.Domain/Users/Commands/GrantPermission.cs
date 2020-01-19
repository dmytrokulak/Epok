using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Grant permission to a user on a domain resource.
    /// </summary>
    public class GrantPermission : CommandBase
    {
        public Guid UserId { get; set; }
        public Guid ResourceId { get; set; }
        public IEnumerable<Guid> Restriction { get; set; }
    }
}
