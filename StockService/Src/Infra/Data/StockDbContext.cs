using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;

namespace StockService.Infra.Data;

public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasIndex(p => p.Code).IsUnique();

            entity.Property(p => p.Code)
                .HasMaxLength(20);

            entity.Property(p => p.Description)
                .HasMaxLength(255);

            entity.Property(p => p.StockBalance);

            entity.Property(p => p.RowVersion).IsRowVersion();

            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}