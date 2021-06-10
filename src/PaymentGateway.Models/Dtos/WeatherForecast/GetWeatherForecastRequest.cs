using System;

namespace PaymentGateway.Models.Dtos.WeatherForecast
{
    public record GetWeatherForecastRequest(DateTime FromDate, DateTime? ToDate = null);
}