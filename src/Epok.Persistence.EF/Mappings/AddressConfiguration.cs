using Epok.Domain.Contacts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.AddressLine1).HasMaxLength(50).IsRequired();
            builder.Property(e => e.AddressLine2).HasMaxLength(50);
            builder.Property(e => e.City).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Province).HasMaxLength(50);
            builder.Property(e => e.Country).HasMaxLength(50);
            builder.Property(e => e.PostalCode).HasMaxLength(50);
        }
    }
}
