using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class BillOfMaterialConfiguration : IEntityTypeConfiguration<BillOfMaterial>
    {
        public void Configure(EntityTypeBuilder<BillOfMaterial> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.HasOne(e => e.Article);
            builder.HasMany(e => e.Input);
            builder.Property(e => e.Output).HasDefaultValue(1).IsRequired();
            builder.Property(e => e.Primary).HasDefaultValue(false).IsRequired();
        }
    }
}
