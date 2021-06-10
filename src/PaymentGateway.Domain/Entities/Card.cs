using System;

namespace PaymentGateway.Domain.Entities
{
    public class Card : BaseEntity
    {
        public int Cvv { get; set; }

        public string CardNumber { get; set; }

        public string HolderName { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }

        public Account Account { get; set; }
    }
}