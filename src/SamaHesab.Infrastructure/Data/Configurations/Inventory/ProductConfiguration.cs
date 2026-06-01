using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SamaHesab.Domain.Entities.Inventory;
using SamaHesab.Domain.Enums;

namespace SamaHesab.Infrastructure.Data.Configurations.Inventory;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.Property(p => p.Barcode2).HasMaxLength(50);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.NameEn).HasMaxLength(300);
        builder.Property(p => p.ProductType).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.ValuationMethod).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(p => p.SalePrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(p => p.WholesalePrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(p => p.ConsumerPrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(p => p.MinStock).HasColumnType("decimal(18,4)").HasDefaultValue(0m);
        builder.Property(p => p.MaxStock).HasColumnType("decimal(18,4)");
        builder.Property(p => p.TaxRate).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(p => new { p.CompanyId, p.Code }).IsUnique();
        builder.HasIndex(p => p.Barcode).HasFilter("[Barcode] IS NOT NULL");

        builder.HasMany(p => p.StockItems)
               .WithOne(s => s.Product)
               .HasForeignKey(s => s.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.PriceLevels)
               .WithOne()
               .HasForeignKey(pl => pl.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Code).IsRequired().HasMaxLength(20);
        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Address).HasMaxLength(500);
        builder.Property(w => w.ManagerName).HasMaxLength(100);
        builder.Property(w => w.CreatedAt).HasDefaultValueSql("GETDATE()");
        builder.HasIndex(w => new { w.CompanyId, w.Code }).IsUnique();

        builder.HasMany(w => w.StockItems)
               .WithOne(s => s.Warehouse)
               .HasForeignKey(s => s.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Quantity).HasColumnType("decimal(18,4)").HasDefaultValue(0m);
        builder.Property(s => s.AverageCost).HasColumnType("decimal(18,4)").HasDefaultValue(0m);
        builder.Property(s => s.LastCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.LastUpdated).HasDefaultValueSql("GETDATE()");
        builder.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique();
    }
}
