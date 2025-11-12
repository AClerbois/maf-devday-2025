using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var existingFoundryName = builder.AddParameter("existingFoundryName", "devday-2025-maf");
var existingFoundryResourceGroup = builder.AddParameter("existingFoundryResourceGroup", "rg-devday");

var azureFoundry = builder
    .AddAzureAIFoundry("devday-2025-maf")
    .AsExisting(existingFoundryName, existingFoundryResourceGroup);

var chatModel = azureFoundry.AddDeployment("chat-demo", AIFoundryModel.OpenAI.Gpt4o);

var apiService = builder.AddProject<Projects.AiAgentAspireApp_ApiService>("apiservice")
    .WithReference(chatModel)
    .WaitFor(chatModel)
    .WithHttpHealthCheck("/health");

var devdaycontentmcp = builder.AddProject<Projects.DevDayContentMcp>("devdaycontentmcp");

var webapp = builder.AddProject<Projects.AiAgentAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(devdaycontentmcp)
    .WaitFor(devdaycontentmcp)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(chatModel)
    .WaitFor(chatModel);

builder.Build().Run();
