using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Suppliers.Entities;
using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Epok.Persistence.EF
{
    public class DomainContext : DbContext
    {
        private readonly IConfiguration _config;

        public DomainContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<BillOfMaterial> BillsOfMaterial { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<DomainResource> DomainResources { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<MaterialRequest> MaterialRequests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ShopCategory> ShopCategories { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<SpoilageReport> SpoilageReports { get; set; }
        public DbSet<SpoiledArticle> SpoiledArticles { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Uom> Uoms { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseNpgsql(_config.GetConnectionString("ErpDb"));
        }
    }
}
