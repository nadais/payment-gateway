using System;

namespace PaymentGateway.Models.Dtos.WeatherForecast
{
    public record WeatherForecastDto
    {
        public DateTime Date { get; init; }

        public int TemperatureC { get; init; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        public string Summary { get; init; }
    }
}