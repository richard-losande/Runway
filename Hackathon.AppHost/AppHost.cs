var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Hackathon_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Hackathon_Bff>("bff")
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddNpmApp("frontend", "../Hackathon.Frontend", "dev")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithHttpEndpoint(env: "PORT");

builder.Build().Run();
