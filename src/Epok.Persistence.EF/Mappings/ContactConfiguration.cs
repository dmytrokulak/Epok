using Epok.Domain.Contacts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.FirstName).HasMaxLength(30).IsRequired();
            builder.Property(e => e.MiddleName).HasMaxLength(30);
            builder.Property(e => e.LastName).HasMaxLength(30);
            builder.Property(e => e.Email).HasMaxLength(50);
            builder.Property(e => e.PhoneNumber).HasMaxLength(20);
            builder.Property(e => e.CompanyId);
            builder.Property(e => e.Primary).HasDefaultValue(false);
        }
    }
}
