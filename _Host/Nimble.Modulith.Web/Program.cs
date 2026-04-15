using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Nimble.Modulith.Users;
using Nimble.Modulith.Products;
using Nimble.Modulith.Customers;
using Nimble.Modulith.Email;
using Nimble.Modulith.Web;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

// Add service defaults (Aspire configuration)
builder.AddServiceDefaults();

// Add Mediator with source generation
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.PipelineBehaviors =
    [
        typeof(LoggingBehavior<,>)
    ];
});

// Add FastEndpoints with JWT Bearer Authentication and Authorization
builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(o => o.SigningKey = builder.Configuration["Auth:JwtSecret"]!)
    .AddAuthorization();

// Add Swagger/OpenAPI
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Nimble Modulith API";
        s.Version = "v1";
    };
});

// Register modules using IHostApplicationBuilder pattern
builder.AddUsersModuleServices(logger);
builder.AddProductsModuleServices(logger);
builder.AddCustomersModuleServices(logger);
builder.AddEmailModuleServices(logger);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseDefaultExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// Configure FastEndpoints
app.UseFastEndpoints();

// Only use Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapDefaultEndpoints();

// Ensure databases are created and migrated
await app.EnsureUsersModuleDatabaseAsync();
await app.EnsureProductsModuleDatabaseAsync();
await app.EnsureCustomersModuleDatabaseAsync();

app.Run();