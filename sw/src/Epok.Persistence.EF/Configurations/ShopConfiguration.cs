using System;
using Epok.Domain.Shops.Entities;
using Epok.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Configurations
{
    public class ShopConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.HasOne(e => e.ShopCategory);
            builder.Property(e => e.IsDefaultForCategory).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.IsEntryPoint).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.IsExitPoint).HasDefaultValue(false).IsRequired();
            builder.HasMany(e => e.Inventory);
            builder.HasMany(e => e.Orders);

            builder.Property<Guid>("ManagerId");
            builder.HasOne(e => e.Manager)
                   .WithOne(e => e.Shop)
                   .HasForeignKey<User>();

        }
    }
}
