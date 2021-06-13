using System;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Domain.Bank
{
    public class BankPaymentRequest
    {
        public CardRequest FromCard { get; set; }

        public string Currency { get; set; }

        public decimal Quantity { get; set; }

        public Guid ToAccountId { get; set; }
    }
}