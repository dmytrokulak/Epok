using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Modifies a given user.
    /// </summary>
    public class ChangeUserData : CommandBase
    {
        public Guid Id { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
