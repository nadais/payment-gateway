using System;

namespace PaymentGateway.Models.Payments
{
    public record CreatePaymentRequest
    {
        public Guid CardId { get; set; }

        public Guid ShopperId { get; set; }

        public int Cvv { get; set; }

        public string Currency { get; set; }

        public decimal Quantity { get; set; }
    }
}