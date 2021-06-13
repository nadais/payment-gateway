using System;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Extensions.Authentication;

namespace PaymentGateway.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetShopperId()
        {
            return ShopperIdPolicy.GetShopperId(User);
        }
    }
}