using System;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Domain.Bank
{
    public record BankPaymentRequest
    {
        public CardRequest FromCard { get; init; }

        public string Currency { get; init; }

        public decimal Amount { get; init; }

        public Guid ToAccountId { get; init; }
    }
}