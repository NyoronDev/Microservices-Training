using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogApi.Domain;

namespace ProductCatalogApi.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CatalogBrand>(ConfigureCatalogBrand);
            builder.Entity<CatalogType>(ConfigureCatalogType);
            builder.Entity<CatalogItem>(ConfigureCatalogItem);
        }

        private void ConfigureCatalogItem(EntityTypeBuilder<CatalogItem> catalogItemBuilder)
        {
            catalogItemBuilder.ToTable("Catalog");

            catalogItemBuilder.Property(m => m.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired();

            catalogItemBuilder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(50);

            catalogItemBuilder.Property(m => m.Price)
                .IsRequired();

            catalogItemBuilder.Property(m => m.PictureUrl)
                .IsRequired(false);

            catalogItemBuilder.HasOne(m => m.CatalogBrand)
                .WithMany()
                .HasForeignKey(m => m.CatalogBrandId);

            catalogItemBuilder.HasOne(m => m.CatalogType)
                .WithMany()
                .HasForeignKey(m => m.CatalogTypeId);
        }

        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> catalogTypeBuilder)
        {
            catalogTypeBuilder.ToTable("CatalogType");

            catalogTypeBuilder.Property(m => m.Id)
                .UseHiLo("catalog_type_hilo")
                .IsRequired();

            catalogTypeBuilder.Property(m => m.Type)
                .IsRequired()
                .HasMaxLength(100);
        }

        private void ConfigureCatalogBrand(EntityTypeBuilder<CatalogBrand> catalogBrandBuilder)
        {
            catalogBrandBuilder.ToTable("CatalogBrand");

            catalogBrandBuilder.Property(m => m.Id)
                .UseHiLo("catalog_brand_hilo")
                .IsRequired();

            catalogBrandBuilder.Property(m => m.Brand)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
