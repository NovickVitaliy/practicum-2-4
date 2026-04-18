using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data.Config;

public class FactOrderConfig : IEntityTypeConfiguration<FactOrder>
{
    public void Configure(EntityTypeBuilder<FactOrder> builder)
    {
        builder.ToTable("FactOrders", "Reporting");

        // ✅ Composite PK (grain = 1 row per order item)
        builder.HasKey(x => new { x.OrderNumber, x.OrderItemId });

        // ✅ Degenerate dimensions
        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OrderItemId)
            .IsRequired();

        // ✅ Measures
        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.OrderTotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        // ✅ Relationships (Fact → Dimensions)
        builder.HasOne(x => x.DimDate)
            .WithMany(d => d.FactOrders)
            .HasForeignKey(x => x.DimDateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DimCustomer)
            .WithMany(c => c.FactOrders)
            .HasForeignKey(x => x.DimCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DimProduct)
            .WithMany(p => p.FactOrders)
            .HasForeignKey(x => x.DimProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // ✅ Indexes (CRITICAL for reporting performance)
        builder.HasIndex(x => x.DimDateId);
        builder.HasIndex(x => x.DimCustomerId);
        builder.HasIndex(x => x.DimProductId);

        // Optional but useful for aggregations
        builder.HasIndex(x => x.OrderNumber);
    }
}