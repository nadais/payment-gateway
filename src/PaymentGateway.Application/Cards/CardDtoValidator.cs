using System.Linq;
using FluentValidation;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardDtoValidator : AbstractValidator<CardDto>
    {
        public CardDtoValidator()
        {
            RuleFor(x => x.ExpirationMonth)
                .GreaterThan(0);
            RuleFor(x => x.ExpirationYear)
                .GreaterThan(0);
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .Must(x => x.ToCharArray().All(c => c - '0' <= 9 ))
                .WithMessage("All characters in cardNumber must be digits");
        }
    }
}