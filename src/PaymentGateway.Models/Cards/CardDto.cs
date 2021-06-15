using System;

namespace PaymentGateway.Models.Cards
{
    public record CardDto
    {
        public Guid Id { get; init; }
        
        public string CardNumber { get; init; }

        public string HolderName { get; init; }

        public DateTimeOffset CreatedAt { get; init; }
    }
}