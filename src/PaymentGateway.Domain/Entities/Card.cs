namespace PaymentGateway.Domain.Entities
{
    public class Card : BaseEntity
    {
        public string Cvv { get; set; }

        public string CardNumber { get; set; }

        public string HolderName { get; set; }

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}