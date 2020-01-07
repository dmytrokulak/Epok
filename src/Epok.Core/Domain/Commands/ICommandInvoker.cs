using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Core.Domain.Commands
{
    /// <summary>
    /// Uses IoC container as a service locator to decide on
    /// the command handler to handle the command passed.
    /// Should have IoC container injected as dependency.
    /// </summary>
    public interface ICommandInvoker
    {
        /// <summary>
        /// Executes a single command.
        /// </summary>
        Task Execute<T>(T command) where T : ICommand;

        /// <summary>
        /// Executes multiple commands within a single unit of work.
        /// </summary>
        Task Execute<T>(IEnumerable<T> commands) where T : ICommand;
    }
}
