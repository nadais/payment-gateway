using System;

namespace PaymentGateway.Models.Dtos.Cards
{
    public record CardDto
    {
        public Guid Id { get; init; }

        public int Cvv { get; init; }

        public string CardNumber { get; init; }

        public string HolderName { get; init; }

        public int ExpirationMonth { get; init; }

        public int ExpirationYear { get; init; }

        public Guid ShopperId { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}