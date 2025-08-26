var builder = DistributedApplication.CreateBuilder(args);

var rg = builder.AddParameter("ResourceGroupName");
var foundryResourceName = builder.AddParameter("FoundryResourceName");

var foundry = builder.AddAzureAIFoundry("ai-foundry").RunAsExisting(foundryResourceName, rg);

var apiService = builder.AddProject<Projects.AspireApp25_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(foundry)
    .WaitFor(foundry)
    .WithEnvironment(async ctx =>
    {
        var foundryConnectionString = await foundry.Resource.ConnectionStringExpression.GetValueAsync(default);
    });

builder.AddProject<Projects.AspireApp25_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
