using FluentValidation;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardRequestValidator : AbstractValidator<CardRequest>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public CardRequestValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            RuleFor(x => x.ExpirationMonth)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12);
            RuleFor(x => x.ExpirationYear)
                .GreaterThanOrEqualTo(0)
                .LessThan(100);
            RuleFor(x => x)
                .Must(IsCardValid)
                .WithMessage("The provided card is expired");
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .CreditCard();
        }

        private bool IsCardValid(CardRequest request)
        {
            var currentTime = _dateTimeProvider.GetCurrentTime();
            var last2DigitsYear = currentTime.Year % 100;
            var month = currentTime.Month;
            var isYearInFuture = request.ExpirationYear > last2DigitsYear;
            if (last2DigitsYear >= 95)
            {
                isYearInFuture = isYearInFuture || request.ExpirationYear <= 10;
            }

            return isYearInFuture ||
                   request.ExpirationYear == last2DigitsYear && request.ExpirationMonth >= month;
        }
    }
}