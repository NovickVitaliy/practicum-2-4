var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var usersDb = sqlServer.AddDatabase("usersdb");
var productsDb = sqlServer.AddDatabase("productsdb");
var customersDb = sqlServer.AddDatabase("customersdb");

builder.AddProject<Projects.Nimble_Modulith_Web>("webapi")
    .WithReference(usersDb)
    .WithReference(productsDb)
    .WithReference(customersDb)
    .WaitFor(usersDb)
    .WaitFor(productsDb)
    .WaitFor(customersDb);

builder.Build().Run();