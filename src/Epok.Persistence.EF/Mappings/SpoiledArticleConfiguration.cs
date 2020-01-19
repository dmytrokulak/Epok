using Epok.Domain.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epok.Persistence.EF.Mappings
{
    public class SpoiledArticleConfiguration : IEntityTypeConfiguration<SpoiledArticle>
    {

        public void Configure(EntityTypeBuilder<SpoiledArticle> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Fixable).HasDefaultValue(false).IsRequired();
            builder.Property(e => e.Reusable).HasDefaultValue(false).IsRequired();
            builder.HasOne(e => e.Article);
        }
    }
}
