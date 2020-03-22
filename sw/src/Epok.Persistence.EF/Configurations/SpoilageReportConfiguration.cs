using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class SpoilageReportConfiguration : IEntityTypeConfiguration<SpoilageReport>
    {
        public void Configure(EntityTypeBuilder<SpoilageReport> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Reason).HasMaxLength(100).IsRequired();
            builder.Property(e => e.TimeOfReport).IsRequired();
            builder.HasOne(e => e.Item);
        }
    }
}
