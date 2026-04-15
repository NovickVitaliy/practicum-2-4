var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var usersDb = sqlServer.AddDatabase("usersdb");
var productsDb = sqlServer.AddDatabase("productsdb");

builder.AddProject<Projects.Nimble_Modulith_Web>("webapi")
    .WithReference(usersDb)
    .WithReference(productsDb)
    .WaitFor(usersDb)
    .WaitFor(productsDb);

builder.Build().Run();