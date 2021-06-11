using Microsoft.AspNetCore.Authentication;

namespace PaymentGateway.Api.Extensions.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }     
    }
}