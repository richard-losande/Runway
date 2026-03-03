using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using Hackathon.ApiService.Databases.DbContexts;
using Hackathon.ApiService.Features.FinancialRunway;
using Hackathon.ApiService.Infrastractures.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthentication>("ApiKey", _ => { });

var dbConnectionString = builder.Configuration.GetConnectionString("hackathonDb");
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(dbConnectionString));
builder.Services.AddScoped<IMaindbContext>(sp => sp.GetRequiredService<MainDbContext>());
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();
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

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();
app.MapDefaultEndpoints();
app.Run();

