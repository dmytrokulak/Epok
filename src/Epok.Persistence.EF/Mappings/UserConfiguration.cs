using System;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.IsShopManager).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.UserType).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(50).IsRequired();

            builder.Property<Guid>("ShopId");
            builder.HasOne(e => e.Shop)
                .WithOne(e => e.Manager)
                .HasForeignKey<Shop>();
        }
    }
}
