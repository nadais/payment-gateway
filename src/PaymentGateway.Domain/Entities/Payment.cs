using System;
using System.ComponentModel.DataAnnotations;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        
        public Guid CardId { get; set; }

        public string CardNumber { get; set; }

        public Guid ShopperId { get; set; }

        public decimal Quantity { get; set; }

        public string Currency { get; set; }

        public PaymentStatus Status { get; set; }
    }
}