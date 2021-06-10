using System;
using FluentValidation;
using PaymentGateway.Models.Dtos.WeatherForecast;

namespace PaymentGateway.Application.Validators
{
    public class GetWeatherForecastRequestValidator : AbstractValidator<GetWeatherForecastRequest>
    {
        public GetWeatherForecastRequestValidator()
        {
            RuleFor(x => x.FromDate)
                .GreaterThan(DateTime.MinValue);
            RuleFor(x => x.ToDate)
                .Must((request, toDate) => toDate == null || toDate > request.FromDate)
                .WithMessage("To date has to be greater than from date if defined");
        }
    }
}