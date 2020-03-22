using Epok.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.HasMany(e => e.SuppliableArticles);
            builder.HasMany(e => e.MaterialRequests);
            builder.HasOne(e => e.ShippingAddress);
            builder.HasMany(e => e.Contacts);
        }
    }
}
