using FluentValidation;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}