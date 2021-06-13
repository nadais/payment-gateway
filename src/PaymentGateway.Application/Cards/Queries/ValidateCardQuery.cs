using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards.Queries
{
    public record ValidateCardQuery(CardRequest Card) : IRequest<CardValidationResponse>;

    public class ValidateCardQueryHandler : IRequestHandler<ValidateCardQuery, CardValidationResponse>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        public ValidateCardQueryHandler(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public Task<CardValidationResponse> Handle(ValidateCardQuery request, CancellationToken cancellationToken)
        {
            var errors = new Dictionary<string, ICollection<string>>();
            if (IsCardExpired(request))
            {
                AddError(errors, nameof(ValidateCardQuery.Card), "The provided card is expired");
            }
            if (!LuhnAlgorithmCheck(request.Card.CardNumber))
            {
                AddError(errors, nameof(CardDto.CardNumber), "The provided card number is not valid");
            }

            if (errors.Count > 0)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The provided card details are not valid",
                    "INVALID_CARD_DETAILS", null, errors);
            }
            return Task.FromResult(new CardValidationResponse
            {
                IsValid = true
            });
        }
        private bool IsCardExpired(ValidateCardQuery request)
        {
            var currentTime = _dateTimeProvider.GetCurrentTime();
            var last2DigitsYear = currentTime.Year % 100;
            var month = currentTime.Month;
            var isYearInFuture = request.Card.ExpirationYear > last2DigitsYear;
            if (last2DigitsYear >= 95)
            {
                isYearInFuture = isYearInFuture || request.Card.ExpirationYear <= 10;
            }
            return !isYearInFuture ||
                    request.Card.ExpirationYear == last2DigitsYear && request.Card.ExpirationMonth < month;
        }

        private bool LuhnAlgorithmCheck(string creditCardNumber)
        {
            if (creditCardNumber == null)
            {
                return false;
            }
            var sum = 0;
            var alternate = false;
            var digits = creditCardNumber.ToArray();
            for (var i = digits.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(digits[i].ToString());

                if (alternate)
                {
                    digit *= 2;

                    if (digit > 9)
                    {
                        digit = digit % 10 + 1;
                    }
                }
                sum += digit;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }

        private void AddError(IDictionary<string, ICollection<string>> errors, string property, string message)
        {
            if (!errors.ContainsKey(property))
            {
                errors.Add(property, new List<string>());
            }
            errors[property].Add(message);
        }
    }
}