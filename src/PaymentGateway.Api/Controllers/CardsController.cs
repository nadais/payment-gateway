using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Queries.Cards;
using PaymentGateway.Models.Dtos.Cards;

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
    }
}