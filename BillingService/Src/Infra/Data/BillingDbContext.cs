using BillingService.Domain.Entities;
using BillingService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            entity.HasKey(i => i.Id);
            
            var invoiceNoteIdConversor = new ValueConverter<InvoiceNoteId, Guid>(
                v => v.Value,
                v => InvoiceNoteId.From(v));
            
            entity.Property(i => i.Id)
                .HasConversion(invoiceNoteIdConversor)
                .ValueGeneratedNever();
            
            entity.HasIndex(i => i.NoteNumber).IsUnique();
            
            entity.Property(i => i.NoteNumber)
                .ValueGeneratedOnAdd();
            
            entity.Property(i => i.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.Xmin)
                .HasColumnName("xmin")
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
            
            entity.HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InvoiceNoteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceNoteItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            
            entity.Property(i => i.Id).ValueGeneratedOnAdd();

            entity.Property(i => i.ProductCode).HasMaxLength(20);
            
            entity.Property(i => i.ProductDescription).HasMaxLength(255);
        });
    }
}