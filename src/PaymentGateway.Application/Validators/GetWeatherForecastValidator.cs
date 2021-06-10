using FluentValidation;
using PaymentGateway.Application.Queries;

namespace PaymentGateway.Application.Validators
{
    public class GetWeatherForecastValidator : AbstractValidator<GetWeatherForecastQuery>
    {
        public GetWeatherForecastValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .SetValidator(new GetWeatherForecastRequestValidator());
        }
    }
}