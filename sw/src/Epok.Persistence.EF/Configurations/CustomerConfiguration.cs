using Epok.Domain.Contacts.Entities;
using Epok.Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CustomerType).IsRequired();
            builder.HasOne(e => e.ShippingAddress).WithOne()
                .HasForeignKey<Address>(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Contacts).WithOne()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Orders);
        }
    }
}
