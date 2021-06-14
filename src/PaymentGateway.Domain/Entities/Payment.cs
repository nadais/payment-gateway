using System;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        
        public Guid CardId { get; set; }

        public Card Card { get; set; }

        public Guid? ExternalId { get; set; }

        public Guid ShopperId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Key { get; set; }

        public PaymentStatus Status { get; set; }
    }
}