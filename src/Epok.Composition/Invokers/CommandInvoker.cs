using System.Collections.Generic;
using System.Threading.Tasks;
using Epok.Core.Domain.Commands;
using Epok.Core.Persistence;

namespace Epok.Composition.Invokers
{
    public class CommandInvoker : ICommandInvoker
    {
        private readonly IUnitOfWorkFactory<IUnitOfWork> _work;

        public CommandInvoker(IUnitOfWorkFactory<IUnitOfWork> work)
        {
            _work = work;
        }

        public async Task Execute<T>(T command) where T : ICommand
        {
            using (_work.Transact())
            {
                var handler = Root.Container.GetInstance<ICommandHandler<T>>();
                await handler.HandleAsync(command);
            }
        }

        public async Task Execute<T>(IEnumerable<T> commands) where T : ICommand
        {
            using (_work.Transact())
            {
                //ToDo:3 how to rollback events from the transmitter?
                foreach (var command in commands)
                {
                    var handler = Root.Container.GetInstance<ICommandHandler<T>>();
                    await handler.HandleAsync(command);
                }
            }
        }
    }
}
