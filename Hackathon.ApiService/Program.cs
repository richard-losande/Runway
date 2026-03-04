using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using Hackathon.ApiService.Databases.DbContexts;
using Hackathon.ApiService.Features.FinancialRunway;
using Hackathon.ApiService.Features.Runway.Services;
using Hackathon.ApiService.Features.RunwayV4.Services;
using Hackathon.ApiService.Infrastractures.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
});
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthentication>("ApiKey", _ => { });

var dbConnectionString = builder.Configuration.GetConnectionString("hackathonDb");
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(dbConnectionString));
builder.Services.AddScoped<IMaindbContext>(sp => sp.GetRequiredService<MainDbContext>());
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>()
    .RemoveAllResilienceHandlers()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromMinutes(3));

// Runway Superpowers services
builder.Services.AddHttpClient<ITransactionIntelligence, TransactionIntelligence>()
    .RemoveAllResilienceHandlers()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromMinutes(3));
builder.Services.AddHttpClient<IBehavioralIntelligence, BehavioralIntelligence>()
    .RemoveAllResilienceHandlers()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddScoped<ISurvivalSimulator, SurvivalSimulator>();
builder.Services.AddScoped<IRunwayOrchestrator, RunwayOrchestrator>();

// RunwayV4 services
builder.Services.AddSingleton<IRunwayEngine, RunwayEngine>();
builder.Services.AddScoped<IRunwayV4Orchestrator, RunwayV4Orchestrator>();
builder.Services.AddHttpClient<IDiagnosisNarrativeAgent, DiagnosisNarrativeAgent>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    var apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
});

// CORS for Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
      o.DocumentSettings = s =>
      {
        s.Title = "Report Management API";
        s.Version = "v1";

        s.AddAuth("ApiKey", new OpenApiSecurityScheme
        {
          Type = OpenApiSecuritySchemeType.ApiKey,
          Name = "x-api-key",
          In = OpenApiSecurityApiKeyLocation.Header,
          Description = "Enter API Key"
        });
      };
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
  app.UseDeveloperExceptionPage();
else
  app.UseExceptionHandler();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();
app.MapDefaultEndpoints();
app.Run();

