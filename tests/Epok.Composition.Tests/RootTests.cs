using System.Linq;
using System.Reflection;
using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Queries;
using Epok.Core.Domain.Services;
using Epok.Core.Persistence;
using Epok.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleInjector;

namespace Epok.Composition.Tests
{
    [TestFixture]
    public class RootTests
    {
        [SetUp]
        public void Setup()
        {
            typeof(Root).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(f => f.FieldType == typeof(Container))
                .SetValue(null, new Container());
        }

        [Test]
        [Description("Verified internally by Container.Verify().")]
        public void ContainerShouldRegisterServices()
        {
            Assert.DoesNotThrow(() => Root.InitializeContainer(new DbContextOptions<DomainContext>()));

            var registrations = Root.Container.GetCurrentRegistrations();

            var repositories = registrations
                .Where(r => typeof(IRepository).IsAssignableFrom(r.ServiceType))
                .ToList();
            Assert.That(repositories.Count, Is.EqualTo(5));

            var services = registrations
                .Where(r => typeof(IDomainService).IsAssignableFrom(r.ServiceType))
                .ToList();
            Assert.That(services.Count, Is.EqualTo(2));

            var commandHandlers = registrations
                .Where(r => typeof(ICommandHandler).IsAssignableFrom(r.ServiceType))
                .ToList();
            Assert.That(commandHandlers.Count, Is.EqualTo(38));

            var queryHandlers = registrations
                .Where(r => typeof(IQueryHandler).IsAssignableFrom(r.ServiceType))
                .ToList();
            Assert.That(queryHandlers.Count, Is.EqualTo(4));
        }
    }
}
