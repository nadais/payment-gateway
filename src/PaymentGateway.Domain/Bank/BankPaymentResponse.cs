using System;

namespace PaymentGateway.Domain.Bank
{
    public record BankPaymentResponse
    {
        public Guid? Id { get; init; }

        public bool IsSuccessful { get; set; }
    }
}