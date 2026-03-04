using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using Hackathon.ApiService.Features.FinancialRunway;
using Hackathon.ApiService.Features.Runway.Services;
using Hackathon.ApiService.Features.RunwayV4.Services;
using Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;
using Hackathon.ApiService.Infrastractures.Authentication;
using Hackathon.ApiService.Integrations.SproutPayroll;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Hackathon.ApiService.Databases.DbContexts;
using Microsoft.EntityFrameworkCore;
using Refit;

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

// Database
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("hackathonDb")));
builder.Services.AddScoped<IMaindbContext>(sp => sp.GetRequiredService<MainDbContext>());

// FinancialRunway services
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    var apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

// Runway services
builder.Services.AddSingleton<ISurvivalSimulator, SurvivalSimulator>();
builder.Services.AddHttpClient<ITransactionIntelligence, TransactionIntelligence>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    var apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});
builder.Services.AddHttpClient<IBehavioralIntelligence, BehavioralIntelligence>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    var apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});
builder.Services.AddScoped<IRunwayOrchestrator, RunwayOrchestrator>();

// RunwayV4 services
builder.Services.AddSingleton<IRunwayEngine, RunwayEngine>();
builder.Services.AddSingleton<IFormatDetector, FormatDetector>();
builder.Services.AddSingleton<ITransactionNormalizer, TransactionNormalizer>();
builder.Services.AddSingleton<IMerchantLookup, MerchantLookup>();
builder.Services.AddSingleton<IAggregator, Aggregator>();
builder.Services.AddSingleton<IScenarioGenerator, ScenarioGenerator>();
builder.Services.AddSingleton<IInsightProfileBuilder, InsightProfileBuilder>();
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
builder.Services.AddHttpClient<ICategorizationAgent, CategorizationAgent>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    var apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
});

// Sprout Payroll Refit client
builder.Services.AddTransient<SproutPayrollDelegatingHandler>();
var sproutBaseUrl = builder.Configuration["SproutPayroll:BaseUrl"]
    ?? throw new InvalidOperationException("Missing config: SproutPayroll:BaseUrl");
builder.Services
    .AddRefitClient<ISproutPayrollClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(sproutBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<SproutPayrollDelegatingHandler>();

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

