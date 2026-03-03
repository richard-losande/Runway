using FastEndpoints;
using FastEndpoints.Swagger;
using Hackathon.Bff.Integrations.ApiService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Refit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

var configurations = builder.Configuration;

var kcBase = configurations["KeyCloak:BaseUrl"]?.TrimEnd('/')
    ?? throw new InvalidOperationException("Missing config: KeyCloak:BaseUrl");

var requireHttps = !kcBase.Contains("127.0.0.1") && !kcBase.Contains("localhost");
var expectedIssuerPrefix = $"{kcBase}/realms/";
builder.Services.AddSingleton<KeycloakOidcManagerCache>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.RequireHttpsMetadata = requireHttps;

      options.Events = new JwtBearerEvents
      {

        OnMessageReceived = context =>
        {
          var token = ExtractBearerOrQueryToken(context);
          if (string.IsNullOrWhiteSpace(token))
          {
            context.NoResult();
            return Task.CompletedTask;
          }

          context.Token = token;
          return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
          var issuer = context.SecurityToken.Issuer;

          if (!issuer.StartsWith(expectedIssuerPrefix, StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenInvalidIssuerException("Invalid issuer.");

          var tenant = ExtractRealmFromIssuer(issuer, kcBase);

          var identity = (ClaimsIdentity)context.Principal!.Identity!;
          if (!identity.HasClaim(c => c.Type == "tenant"))
            identity.AddClaim(new Claim("tenant", tenant));

          return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
          context.NoResult();
          context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
          return Task.CompletedTask;
        }
      };

      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,

        IssuerValidator = (issuer, token, parameters) =>
        {
          if (string.IsNullOrWhiteSpace(issuer))
            throw new SecurityTokenInvalidIssuerException("Missing issuer.");

          if (!issuer.StartsWith(expectedIssuerPrefix, StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenInvalidIssuerException("Invalid issuer.");

          return issuer;
        },

        ValidateAudience = true,
        ValidAudience = "account",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
          var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
          var issuer = jwt.Issuer;

          if (string.IsNullOrWhiteSpace(issuer) ||
              !issuer.StartsWith(expectedIssuerPrefix, StringComparison.OrdinalIgnoreCase))
          {
            return Enumerable.Empty<SecurityKey>();
          }

          var cache = builder.Services.BuildServiceProvider()
              .GetRequiredService<KeycloakOidcManagerCache>();

          var oidc = cache.GetAsync(issuer, requireHttps, CancellationToken.None)
              .GetAwaiter().GetResult();

          return oidc.SigningKeys;
        },

        NameClaimType = "preferred_username",
        ClockSkew = TimeSpan.FromMinutes(1)
      };
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true);
  });
});

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
      o.DocumentSettings = s =>
      {
        s.Title = "BFF API";
        s.Version = "v1";
      };
    });
builder.Services.AddTransient<ApiKeyDelegatingHandler>();
builder.Services
    .AddRefitClient<IApiServiceClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https+http://apiservice"))
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
  app.UseSwaggerGen();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.MapDefaultEndpoints();

app.Run();
static string ExtractRealmFromIssuer(string issuer, string kcBase)
{
  var prefix = $"{kcBase.TrimEnd('/')}/realms/";
  if (!issuer.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
    throw new SecurityTokenInvalidIssuerException("Invalid issuer.");

  return issuer.Substring(prefix.Length).Trim('/');
}
static string? ExtractBearerOrQueryToken(MessageReceivedContext context)
{
  if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
  {
    var s = authHeader.ToString();
    if (s.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      return s["Bearer ".Length..].Trim();
  }

  // SignalR JS uses ?access_token=... for WebSockets
  var accessToken = context.Request.Query["access_token"];
  return StringValues.IsNullOrEmpty(accessToken) ? null : accessToken.ToString();
}
public sealed class KeycloakOidcManagerCache
{
  private readonly ConcurrentDictionary<string, IConfigurationManager<OpenIdConnectConfiguration>> _managers = new();

  public Task<OpenIdConnectConfiguration> GetAsync(string authority, bool requireHttps, CancellationToken ct)
  {
    var manager = _managers.GetOrAdd(authority, auth =>
    {
      var retriever = new HttpDocumentRetriever { RequireHttps = requireHttps };
      return new ConfigurationManager<OpenIdConnectConfiguration>(
          $"{auth.TrimEnd('/')}/.well-known/openid-configuration",
          new OpenIdConnectConfigurationRetriever(),
          retriever);
    });

    return manager.GetConfigurationAsync(ct);
  }
}