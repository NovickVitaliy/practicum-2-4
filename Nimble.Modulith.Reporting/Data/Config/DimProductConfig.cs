using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data.Config;

public class DimProductConfig : IEntityTypeConfiguration<DimProduct>
{
    public void Configure(EntityTypeBuilder<DimProduct> builder)
    {
        builder.ToTable("DimProduct");
        builder.HasKey(x => x.ProductId);

        builder.Property(x => x.ProductId)
            .ValueGeneratedNever();
        
        builder.Property(x => x.ProductName)
            .HasMaxLength(100)
            .IsRequired();
    }
}