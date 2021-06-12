using System;

namespace PaymentGateway.Models.Payments
{
    public record PaymentDto
    {
        public Guid Id { get; init; }
        
        public Guid ShopperId { get; init; }
        
        public string Currency { get; init; }

        public decimal Quantity { get; init; }
        
        public Guid CardId { get; init; }

        public string CardNumber { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

    }
}