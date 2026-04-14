var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var usersDb = sqlServer.AddDatabase("usersdb");

builder.AddProject<Projects.Nimble_Modulith_Web>("webapi")
    .WithReference(usersDb)
    .WaitFor(usersDb);

builder.Build().Run();