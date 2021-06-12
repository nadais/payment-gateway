using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Cards.Commands;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/cards")]
    [Authorize]
    public class CardsController
    {
        private readonly IMediator _mediator;
        public CardsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("validation")]
        public async Task<CardValidationResponse> ValidateCard([FromBody] CardDto request, CancellationToken cancellationToken = default)
        {
             return await _mediator.Send(new ValidateCardQuery(request), cancellationToken);
        }
        
        [HttpPost]
        public async Task<CardDto> CreateCard([FromBody] CreateCardRequest request, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new CreateCardCommand(request), cancellationToken);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<CardDto> GetCardById([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new GetCardByIdQuery(id), cancellationToken);
        }
    }
}