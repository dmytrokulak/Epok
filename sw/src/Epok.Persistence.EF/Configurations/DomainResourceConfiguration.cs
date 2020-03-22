using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class DomainResourceConfiguration : IEntityTypeConfiguration<DomainResource>
    {
        public void Configure(EntityTypeBuilder<DomainResource> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.KeyEntityName).HasMaxLength(20);
        }
    }
}
