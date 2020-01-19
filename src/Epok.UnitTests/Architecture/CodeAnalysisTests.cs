using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Queries;
using Epok.Core.Domain.Services;
using Epok.Core.Providers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using Epok.Core.Persistence;

namespace Epok.UnitTests.Architecture
{
    [TestFixture]
    public class CodeAnalysisTests
    {
        [Test]
        public void ShouldAvoidOverinjectionOfDependencies()
        {
            const int maxDependenciesCount = 4;

            var queryInfo = Assembly.LoadFrom("Epok.Domain.dll")
                .GetTypes()
                .SelectMany(t => t.GetConstructors())
                .Where(c => c.GetParameters().Length > maxDependenciesCount)
                .Select(c => $"{c.DeclaringType.Name}: {c.GetParameters().Length}")
                .ToList();

            if (queryInfo.Count != 0)
                throw new Exception($"There are classes with more than allowed dependency count of " +
                                    $"{maxDependenciesCount}. {Environment.NewLine}" +
                                    $"{queryInfo.Aggregate((cur, nxt) => $"{cur}{Environment.NewLine}{nxt}")}");
        }

        [Test]
        public void HandlersShouldNotDependOnOtherHandlers()
        {
            bool IsHandlerType(Type t)
                => typeof(ICommandHandler).IsAssignableFrom(t)
                   || typeof(IQueryHandler).IsAssignableFrom(t)
                   || typeof(IEventHandler).IsAssignableFrom(t);

            var queryInfo = Assembly.LoadFrom("Epok.Domain.dll")
                .GetTypes()
                .Where(IsHandlerType)
                .SelectMany(t => t.GetConstructors())
                .Where(c => c.GetParameters().Any(p => IsHandlerType(p.ParameterType)))
                .Select(c => $"{c.DeclaringType?.Name}")
                .ToList();

            if (queryInfo.Count != 0)
                throw new Exception($"There are handlers which depend on other handlers. {Environment.NewLine}" +
                                    $"{queryInfo.Aggregate((cur, nxt) => $"{cur}{Environment.NewLine}{nxt}")}");
        }

        [Test]
        public void ServicesCanOnlyDependOnProvidersOrRepositoriesFromTheSameSubdomain()
        {
            bool IsIllegalParameter(ParameterInfo p, ConstructorInfo c)
                => !((typeof(IRepository).IsAssignableFrom(p.ParameterType)
                      && p.ParameterType.Namespace?.Replace(".Repositories", "") ==
                      c.DeclaringType.Namespace.Replace(".Services", ""))
                     || typeof(ICrossCuttingProvider).IsAssignableFrom(p.ParameterType));


            var queryInfo = Assembly.LoadFrom("Epok.Domain.dll")
                .GetTypes()
                .Where(t => typeof(IDomainService).IsAssignableFrom(t))
                .SelectMany(t => t.GetConstructors())
                .Where(c => c.GetParameters().Any(p => IsIllegalParameter(p, c)))
                .Select(c => $"{c.DeclaringType?.Name}")
                .ToList();

            if (queryInfo.Count != 0)
                throw new Exception($"There are services with illegal dependencies. {Environment.NewLine}" +
                                    $"{queryInfo.Aggregate((cur, nxt) => $"{cur}{Environment.NewLine}{nxt}")}");
        }
    }
}
