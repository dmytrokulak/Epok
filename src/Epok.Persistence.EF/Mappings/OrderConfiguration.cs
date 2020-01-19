using Epok.Domain.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.HasOne(e => e.Customer);
            builder.HasMany(e => e.ItemsOrdered);
            builder.HasMany(e => e.ItemsProduced);
            builder.Property(e => e.EstimatedCompletionAt);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.WorkStartedAt);
            builder.Property(e => e.ShippedAt);
            builder.Property(e => e.ShipmentDeadline).IsRequired();
            builder.HasMany(e => e.SubOrders);
            builder.HasOne(e => e.ParentOrder);
            builder.HasOne(e => e.ReferenceOrder);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.Type).IsRequired();
            builder.HasOne(e => e.Shop);
        }
    }
}
