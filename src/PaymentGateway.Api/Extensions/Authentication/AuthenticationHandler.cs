using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PaymentGateway.Api.Extensions.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public const string ApiKeyHeader = "X-API-KEY";
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Options.ApiKey))
                {
                    return AuthenticateResult.NoResult();
                }

                if (Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) == false || apiKey.Count == 0)
                {
                    return AuthenticateResult.NoResult();
                }

                if (string.Equals(apiKey[0], Options.ApiKey, StringComparison.Ordinal))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, Options.ApiKey),
                        new Claim(ClaimTypes.Name, "api-key")
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }
            catch
            {
                return AuthenticateResult.Fail($"Invalid or missing {ApiKeyHeader} header");
            }

            return AuthenticateResult.Fail($"Invalid or missing {ApiKeyHeader} header");
        }
    }
}