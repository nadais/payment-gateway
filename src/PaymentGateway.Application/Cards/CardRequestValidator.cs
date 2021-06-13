using FluentValidation;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardRequestValidator : AbstractValidator<CardRequest>
    {
        public CardRequestValidator()
        {
            RuleFor(x => x.ExpirationMonth)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12);
            RuleFor(x => x.ExpirationYear)
                .GreaterThanOrEqualTo(0)
                .LessThan(100);
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .CreditCard();
        }
    }
}