using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects;

namespace StockService.Infra.Data;

public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            var productIdConverter = new ValueConverter<ProductId, Guid>(
                v => v.Value,
                v => ProductId.From(v));

            entity.Property(p => p.Id)
                .HasConversion(productIdConverter)
                .ValueGeneratedNever();

            entity.HasIndex(p => p.Code).IsUnique();

            entity.Property(p => p.Code)
                .HasMaxLength(20);

            entity.Property(p => p.Description)
                .HasMaxLength(255);

            entity.Property(p => p.StockBalance);

            entity.Property(p => p.Xmin)
                .HasColumnName("xmin")
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<IdempotencyKey>(entity =>
        {
            entity.HasKey(e => e.Key);
        });
    }
}