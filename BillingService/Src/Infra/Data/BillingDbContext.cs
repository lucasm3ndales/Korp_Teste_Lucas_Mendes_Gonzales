using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Infra.Data;

public class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
{
    public DbSet<InvoiceNote> InvoiceNotes { get; set; }

    public DbSet<InvoiceNoteItem> InvoiceNoteItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InvoiceNote>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.HasIndex(p => p.NoteNumber).IsUnique();
            
            entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.RowVersion).IsRowVersion();

            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
            
            entity.HasMany(p => p.Items)
                .WithOne()
                .HasForeignKey(item => item.InvoiceNoteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceNoteItem>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Id).ValueGeneratedOnAdd();

            entity.Property(p => p.ProductCode).HasMaxLength(20);
            
            entity.Property(p => p.ProductDescription).HasMaxLength(255);
        });
    }
}