using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PaymentGateway.Api.Extensions.Authentication
{
    public class ShopperIdPolicy
    {
        internal const string ShopperIdClaim = "ShopperId";
        public const string HasShopperIdPolicy = "HasShopperId";

        public record IsShopperRequirement : IAuthorizationRequirement;

        public class IsShopperRequirementHandler : AuthorizationHandler<IsShopperRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                IsShopperRequirement requirement)
            {
                var shopperIdClaim = context.User.Claims.SingleOrDefault(x => x.Type == ShopperIdClaim);
                if (shopperIdClaim == null)
                {
                    return Task.CompletedTask;
                }

                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        
        public static Guid GetShopperId(ClaimsPrincipal user)
        {
            var shopperIdClaim = user.Claims.Single(x => x.Type == ShopperIdClaim);
            return Guid.Parse(shopperIdClaim.Value);
        }

    }
}