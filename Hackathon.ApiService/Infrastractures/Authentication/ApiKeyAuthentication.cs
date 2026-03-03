using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Hackathon.ApiService.Infrastractures.Authentication
{
  public class ApiKeyAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    private readonly IConfiguration _configuration;
    public ApiKeyAuthentication(
            IConfiguration configuration,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
    {
      _configuration = configuration;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var path = Context.Request.Path.Value ?? string.Empty;
      if (IsPublicPath(path))
        return Task.FromResult(AuthenticateResult.Success(GetTicket()));

      if (!Request.Headers.TryGetValue("x-api-key", out var apiKeyHeaderValues))
        return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));

      var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
      if (string.IsNullOrWhiteSpace(providedApiKey))
        return Task.FromResult(AuthenticateResult.Fail("Missing API Key"));

      var expectedApiKey = _configuration["Auth:ApiKey"];
      if (string.IsNullOrWhiteSpace(expectedApiKey))
        return Task.FromResult(AuthenticateResult.Fail("Server API Key not configured"));

      var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
      var expectedBytes = Encoding.UTF8.GetBytes(expectedApiKey);

      if (providedBytes.Length != expectedBytes.Length)
        return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

      if (!CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
        return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

      return Task.FromResult(AuthenticateResult.Success(GetTicket()));
    }

    private static bool IsPublicPath(string path)
    {
      if (path.Equals("/health", StringComparison.OrdinalIgnoreCase)
          || path.Equals("/alive", StringComparison.OrdinalIgnoreCase)
          || path.Equals("/ready", StringComparison.OrdinalIgnoreCase)
          || path.Equals("/metrics", StringComparison.OrdinalIgnoreCase)
          || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
        return true;

      if (path.Equals("/", StringComparison.OrdinalIgnoreCase))
        return true;

      if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        return true;

      if (path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
        return true;

      return false;
    }

    private AuthenticationTicket GetTicket()
    {
      var claims = new[] { new Claim(ClaimTypes.Name, "SproutInternalUser") };
      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      return new AuthenticationTicket(principal, Scheme.Name);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
      Response.StatusCode = StatusCodes.Status401Unauthorized;
      Response.ContentType = "application/json";
      return Response.WriteAsync("{\"error\":\"Invalid or missing API Key\"}");
    }
  }
}
