using Epok.Core.Domain.Commands;

namespace Epok.Domain.Users.Commands
{
    /// <summary>
    /// Creates a user of the system.
    /// </summary>
    public class CreateUser : CreateEntityCommand
    {
        public new string Name => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
