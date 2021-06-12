using FluentValidation;

namespace PaymentGateway.Application.Cards.Queries
{
    public class ValidateCardQueryValidator : AbstractValidator<ValidateCardQuery>
    {
        public ValidateCardQueryValidator()
        {
            RuleFor(x => x.Card)
                .NotEmpty()
                .SetValidator(new CardDtoValidator());
        }
    }
}