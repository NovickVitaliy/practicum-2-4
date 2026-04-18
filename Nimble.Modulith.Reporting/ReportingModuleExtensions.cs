using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.Services;
using ILogger = Serilog.ILogger;

namespace Nimble.Modulith.Reporting;

public static class ReportingModuleExtensions
{
    public static IHostApplicationBuilder AddReportingModuleServices(
        this IHostApplicationBuilder builder,
        ILogger logger)
    {
        builder.AddSqlServerDbContext<ReportingDbContext>("reportingdb");

        logger.Information("{Module} module services registered", nameof(ReportingModuleExtensions).Replace("ModuleExtensions", ""));

        builder.Services.AddScoped<IReportService, ReportService>();

        return builder;
    }
    
    public static async Task<WebApplication> EnsureReportingModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReportingDbContext>();
        await context.Database.MigrateAsync();
        return app;
    }
}