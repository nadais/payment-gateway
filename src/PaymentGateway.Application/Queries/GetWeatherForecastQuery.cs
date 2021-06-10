using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentGateway.Application.Abstractions;
using PaymentGateway.Models.Dtos.WeatherForecast;

namespace PaymentGateway.Application.Queries
{
    public record GetWeatherForecastQuery(GetWeatherForecastRequest Request) : IRequest<ICollection<WeatherForecastDto>>;

    public class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, ICollection<WeatherForecastDto>>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public GetWeatherForecastQueryHandler(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<ICollection<WeatherForecastDto>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.Request.FromDate;
            var toDate = request.Request.ToDate ?? _dateTimeProvider.GetCurrentTime();
            var totalDays = (toDate - startDate).TotalDays;
            var rng = new Random();
            ICollection<WeatherForecastDto> weatherForecasts = new List<WeatherForecastDto>();
            for (var i = 0; i < totalDays; i++)
            {
                weatherForecasts.Add(new WeatherForecastDto
                {
                    Date = startDate.AddDays(i),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]

                });
            }
            return Task.FromResult(weatherForecasts);
        }
    };
}