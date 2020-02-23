using System;
using System.Linq;
using System.Reflection;
using Epok.Composition.Invokers;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Queries;
using Epok.Core.Domain.Services;
using Epok.Core.Persistence;
using Epok.Core.Providers;
using Epok.Messaging.RMQ;
using Epok.Persistence.EF;
using Epok.Persistence.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using SimpleInjector;

namespace Epok.Composition
{
    //ToDo:3 review lifetimes. use singleton by default?
    public static class Root
    {
        public static Container Container { get; private set; }

        public static void InitializeContainer(DbContextOptions<DomainContext> dbContextOptions)
        {
            Container = new Container();

            var domain = Assembly.Load("Epok.Domain");
            var persistence = Assembly.Load("Epok.Persistence.EF");

            //ToDo:3 Container.Register<ILoggingProvider, LoggingProvider>();
            //ToDo:3 Container.Register<ISettingsProvider, SettingsProvider>();
            Container.RegisterSingleton<ITimeProvider, TimeProvider>();

            Container.RegisterSingleton(() => new DomainContext(dbContextOptions));
            Container.RegisterSingleton<IEntityIdentifiersKeeper, EntityIdentifiersKeeper>();
            Container.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            Container.RegisterSingleton(typeof(IUnitOfWorkFactory<>), typeof(UnitOfWorkFactory<UnitOfWork>));
            Container.Register<IEntityRepository, EntityRepository>();
            foreach (var type in domain.GetInterfacesImplementing(typeof(IRepository)))
                Container.Register(type, persistence.GetTypes().Single(t => type.IsAssignableFrom(t) && !t.IsInterface));

            Container.RegisterSingleton<IEventTransmitter, EventTransmitter>();

            Container.Register(typeof(ICommandHandler<>), domain);
            Container.Register(typeof(IQueryHandler<,>), domain);
            Container.Register(typeof(IEventHandler<>), domain);
            Container.Register<ICommandInvoker, CommandInvoker>();
            Container.Register<IQueryInvoker, QueryInvoker>();
            foreach (var type in domain.GetInterfacesImplementing(typeof(IDomainService)))
                Container.Register(type, domain.GetTypes().Single(t => type.IsAssignableFrom(t) && !t.IsInterface));

            Container.Verify();
        }

        private static Type[] GetInterfacesImplementing(this Assembly assembly, Type interfaceType) =>
            assembly.GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && t.IsInterface).ToArray();
    }
}
