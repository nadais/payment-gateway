using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Extensions.Authentication;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/cards")]
    [Authorize(Policy = ShopperAuthorizationConstants.HasShopperIdPolicy)]
    public class CardsController : BaseController
    {
        private readonly IMediator _mediator;

        public CardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<CardDto> GetCardById([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new GetCardByIdQuery(id), cancellationToken);
        }
        
        [HttpGet("number/{cardNumber}")]
        public async Task<CardDto> GetCardByNumber([FromRoute] string cardNumber, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new GetCardByNumberQuery(cardNumber), cancellationToken);
        }
    }
}