using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PaymentGateway.Api.Extensions.Authentication
{

    public static class ShopperAuthorizationConstants
    {
        internal const string ShopperIdClaim = "ShopperId";
        public const string HasShopperIdPolicy = "HasShopperId";
    }

    public record ShopperAuthorizationRequirement : IAuthorizationRequirement;

    public class ShopperAuthorizationHandler : AuthorizationHandler<ShopperAuthorizationRequirement>
    {
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ShopperAuthorizationRequirement authorizationRequirement)
        {
            var shopperIdClaim = context.User.Claims.SingleOrDefault(x => x.Type == ShopperAuthorizationConstants.ShopperIdClaim);
            if (shopperIdClaim == null)
            {
                return Task.CompletedTask;
            }

            context.Succeed(authorizationRequirement);
            return Task.CompletedTask;
        }
    }
}