using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain.Entities
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ShopperId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }
        public string Cvv { get; set; }

        public string CardNumber { get; set; }

        public string HolderName { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}