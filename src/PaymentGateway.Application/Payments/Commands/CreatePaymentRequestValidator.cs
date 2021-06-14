using FluentValidation;
using PaymentGateway.Application.Cards;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);
            RuleFor(x => x.Card)
                .SetValidator(new CardRequestValidator(dateTimeProvider));
        }
    }
}