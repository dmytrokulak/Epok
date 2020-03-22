using System.Collections.Generic;
using Epok.Core.Domain.Entities;
using Epok.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Epok.Integration.Tests
{
    internal static class DbManager
    {
        private static DbContextOptions _options;

        private static void Init()
        {
            var connectionString = new ConfigurationBuilder()
                .AddJsonFile(@$"{ServerManager.ServerDirectoryPath}\appsettings.json")
                .Build().GetConnectionString("ErpDb");

             _options = new DbContextOptionsBuilder(new DbContextOptions<DomainContext>())
                .UseNpgsql(connectionString).Options;
        }

        internal static void DropAndCreateDb()
        {
            if (_options == null)
                Init();

            using var dbContext = new DomainContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        internal static void BulkInsert<T>(IEnumerable<T> entities) where T : IEntity
        {
            if (_options == null)
                Init();
            using var dbContext = new DomainContext(_options);
            dbContext.AddRange((IEnumerable<object>)entities);
            dbContext.SaveChanges();
        }
    }
}
