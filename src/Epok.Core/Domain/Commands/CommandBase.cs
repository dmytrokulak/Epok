using System;

namespace Epok.Core.Domain.Commands
{
    /// <summary>
    /// Base class containing properties
    /// common for all commands.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// User who submits the command.
        /// </summary>
        public Guid InitiatorId { get; set; }
    }
}
