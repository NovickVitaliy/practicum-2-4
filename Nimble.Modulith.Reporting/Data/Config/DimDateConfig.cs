using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data.Config;

public class DimDateConfig : IEntityTypeConfiguration<DimDate>
{
    public void Configure(EntityTypeBuilder<DimDate> builder)
    {
        builder.ToTable("DimDate");
        builder.HasKey(x => x.DateKey);
        
        builder.Property(x => x.DateKey)
            .ValueGeneratedOnAdd();
        
        builder.HasMany(x => x.FactOrders)
            .WithOne(x => x.DimDate)
            .HasForeignKey(x => x.DimDateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        var dates = GenerateDateDimension(2025);
        builder.HasData(dates);
    }
    
    private static List<DimDate> GenerateDateDimension(int year)
    {
        var dates = new List<DimDate>();
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            dates.Add(new DimDate
            {
                DateKey = int.Parse(date.ToString("yyyyMMdd")),
                Date = date,
                Year = date.Year,
                Quarter = (date.Month - 1) / 3 + 1,
                Month = date.Month,
                Day = date.Day,
                DayOfWeek = (int)date.DayOfWeek,
                DayName = date.DayOfWeek.ToString(),
                MonthName = date.ToString("MMMM")
            });
        }
        
        return dates;
    }
}