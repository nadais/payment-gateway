using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Queries;
using PaymentGateway.Models.Dtos.WeatherForecast;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastDto>> Get([FromQuery] GetWeatherForecastRequest request)
        {
            return await _mediator.Send(new GetWeatherForecastQuery(null));
        }
    }
}