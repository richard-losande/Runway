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
    .WithHttpEndpoint(port: 5173, env: "PORT");

builder.Build().Run();
