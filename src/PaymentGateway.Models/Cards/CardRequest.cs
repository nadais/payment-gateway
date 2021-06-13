using System;

namespace PaymentGateway.Models.Cards
{
    public record CardRequest
    {
        public int Cvv { get; init; }

        public string CardNumber { get; init; }

        public string HolderName { get; init; }

        public int ExpirationMonth { get; init; }

        public int ExpirationYear { get; init; }
    }
}