var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Hackathon_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Hackathon_Bff>("bff")
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
