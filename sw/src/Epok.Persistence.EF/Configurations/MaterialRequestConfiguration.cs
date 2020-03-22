using Epok.Domain.Suppliers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class MaterialRequestConfiguration : IEntityTypeConfiguration<MaterialRequest>
    {
        public void Configure(EntityTypeBuilder<MaterialRequest> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.HasOne(e => e.Supplier);
            builder.HasMany(e => e.ItemsRequested);
        }
    }
}
