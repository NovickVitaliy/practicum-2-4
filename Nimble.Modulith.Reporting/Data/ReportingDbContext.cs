using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Data;

public class ReportingDbContext : DbContext
{
    public DbSet<DimCustomer> DimCustomers => Set<DimCustomer>();
    public DbSet<DimDate> DimDates => Set<DimDate>();
    public DbSet<DimProduct> DimProducts => Set<DimProduct>();
    public DbSet<FactOrder> FactOrders => Set<FactOrder>();

    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}