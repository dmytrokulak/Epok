using System.Threading.Tasks;

namespace Epok.Core.Domain.Commands
{
    /// <summary>
    /// Marker interface for a domain command handler.
    /// </summary>
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<T> : ICommandHandler where T : ICommand
    {
        Task HandleAsync(T command);
    }
}
