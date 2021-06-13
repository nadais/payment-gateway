using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PaymentGateway.Application.Common.Abstractions;

namespace PaymentGateway.Api.Extensions.Authentication
{
    public class JsonWebTokenGenerator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IConfiguration _config;

        public JsonWebTokenGenerator(IDateTimeProvider dateTimeProvider, IConfiguration config)
        {
            _dateTimeProvider = dateTimeProvider;
            _config = config;
        }

        public string GenerateJsonWebToken(Guid shopperId)    
        {    
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));    
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
    
            var claims = new[] {
                new Claim(ShopperAuthorizationConstants.ShopperIdClaim, shopperId.ToString()),    
            };    
    
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],    
                _config["Jwt:Issuer"],    
                claims,    
                expires:_dateTimeProvider.GetCurrentTime().AddMinutes(120),    
                signingCredentials: credentials);    
    
            return new JwtSecurityTokenHandler().WriteToken(token);    
        }

    }
}