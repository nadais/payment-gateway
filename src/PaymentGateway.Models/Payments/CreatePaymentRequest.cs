using PaymentGateway.Models.Cards;

namespace PaymentGateway.Models.Payments
{
    public record CreatePaymentRequest
    {
        public CardRequest Card { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }
    }
}