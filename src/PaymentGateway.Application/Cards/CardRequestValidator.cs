using FluentValidation;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardRequestValidator : AbstractValidator<CardRequest>
    {
        public CardRequestValidator()
        {
            RuleFor(x => x.ExpirationMonth)
                .GreaterThan(0);
            RuleFor(x => x.ExpirationYear)
                .GreaterThan(0);
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .CreditCard();
        }
    }
}