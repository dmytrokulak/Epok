using System;
using Epok.Domain.Inventory;
using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class UomConfiguration : IEntityTypeConfiguration<Uom>
    {
        public void Configure(EntityTypeBuilder<Uom> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.BasePoints).HasDefaultValue(0).IsRequired();
            builder.Property(e => e.Custom).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.Type).IsRequired();

            builder.HasData(new[]
            {
                new Uom(Guid.Parse("00000000-0000-0000-0008-010510199101"), "Piece", UomType.Piece),
                new Uom(Guid.Parse("00000000-0000-0000-0077-101116101114"), "Meter", UomType.Length),
                new Uom(Guid.Parse("13117097-1141-0103-2077-101116101114"), "Square Meter", UomType.Area),
                new Uom(Guid.Parse("71177098-1050-9903-2077-101116101114"), "Cubic Meter", UomType.Volume),
                new Uom(Guid.Parse("00000000-0000-0000-0000-007111497109"), "Gram", UomType.Weight),
                new Uom(Guid.Parse("00077105-1081-0810-5109-101116101114"), "Millimeter", UomType.Length)
                    {BasePoints = 0.001M},
                new Uom(Guid.Parse("00067101-1101-1610-5109-101116114101"), "Centimeter", UomType.Length)
                    {BasePoints = 0.01M},
                new Uom(Guid.Parse("00000000-0751-0510-8111-103114097109"), "Kilogram", UomType.Weight)
                    {BasePoints = 1000M},
            });
        }
    }
}
