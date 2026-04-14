using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Nimble.Modulith.Users;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.AddServiceDefaults();

builder.AddUsersModuleServices(logger);

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
        s.SigningKey = builder.Configuration["Auth:JwtSecret"];
    })
    .AddAuthorization()
    .SwaggerDocument();

var app = builder.Build();

await app.EnsureUsersModuleDatabaseAsync();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();