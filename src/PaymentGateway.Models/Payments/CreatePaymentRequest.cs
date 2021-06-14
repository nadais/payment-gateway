using System;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Models.Payments
{
    public record CreatePaymentRequest
    {
        public CardRequest Card { get; init; }

        public string Currency { get; init; }

        public decimal Amount { get; init; }

        public DateTimeOffset SentAt { get; init; }
    }
}