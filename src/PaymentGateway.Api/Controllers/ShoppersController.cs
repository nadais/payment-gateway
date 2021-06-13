using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Api.Extensions.Authentication;
using PaymentGateway.Application.Common.Abstractions;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/shoppers")]
    public class ShoppersController : ControllerBase
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IConfiguration _configuration;

        public ShoppersController(IDateTimeProvider dateTimeProvider, IConfiguration configuration)
        {
            _dateTimeProvider = dateTimeProvider;
            _configuration = configuration;
        }

        [HttpGet("{id:guid}/login")]
        public async Task<string> Login([FromRoute] Guid id)
        {
            var token = new JsonWebTokenGenerator(_dateTimeProvider, _configuration);
            return token.GenerateJSONWebToken(id);
        }
    }
}