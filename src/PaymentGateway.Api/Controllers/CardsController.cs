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
        public async Task<CardValidationResponse> ValidateCard([FromBody] CardDto request)
        {
             return await _mediator.Send(new ValidateCardQuery(request));
        }
        
        [HttpPost]
        public async Task<CardDto> CreateCard([FromBody] CreateCardRequest request)
        {
            return await _mediator.Send(new CreateCardCommand(request));
        }
    }
}