using Epok.Core.Domain.Commands;
using System;
using System.Collections.Generic;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Grant permission to a user on a CQRS resource.
    /// </summary>
    public class GrantPermission : CommandBase
    {
        public Guid UserId { get; set; }
        public Guid HandleId { get; set; }
        public IEnumerable<Guid> Restriction { get; set; }
    }
}
