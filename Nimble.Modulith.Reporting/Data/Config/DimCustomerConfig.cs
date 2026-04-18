using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data.Config;

public class DimCustomerConfig : IEntityTypeConfiguration<DimCustomer>
{
    public void Configure(EntityTypeBuilder<DimCustomer> builder)
    {
        builder.ToTable("DimCustomer");
        builder.HasKey(c => c.CustomerId);
        
        builder.Property(c => c.CustomerId)
            .ValueGeneratedNever();

        builder.Property(c => c.Email)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasMany(c => c.FactOrders)
            .WithOne(c => c.DimCustomer)
            .HasForeignKey(c => c.DimCustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}