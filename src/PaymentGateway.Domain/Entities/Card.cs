using System;

namespace PaymentGateway.Domain.Entities
{
    public class Card
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string CardNumber { get; set; }

        public string HolderName { get; set; }
    }
}