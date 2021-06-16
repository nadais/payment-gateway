using System;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Models.Payments
{
    public record PaymentDto
    {
        public Guid Id { get; init; }
        
        public Guid ShopperId { get; init; }
        
        public string Currency { get; init; }

        public decimal Amount { get; init; }

        public Guid CardId { get; init; }

        public CardDto Card { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public PaymentStatus Status { get; init; }

    }
}