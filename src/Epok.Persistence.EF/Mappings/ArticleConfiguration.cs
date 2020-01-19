using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.ArticleType).IsRequired();
            builder.Property(e => e.Code).HasMaxLength(10).IsRequired();
            builder.HasOne(e => e.UoM);
            builder.HasMany(e => e.BillsOfMaterial);
            builder.HasOne(e => e.ProductionShopCategory);
            builder.Property(e => e.TimeToProduce);
        }
    }
}
