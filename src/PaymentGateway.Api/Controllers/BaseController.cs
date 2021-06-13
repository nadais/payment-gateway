using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Extensions.Authentication;

namespace PaymentGateway.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetShopperId()
        {
            var shopperIdClaim = User.Claims.Single(x => x.Type == ShopperAuthorizationConstants.ShopperIdClaim);
            return Guid.Parse(shopperIdClaim.Value);
        }
    }
}