using System;

namespace PaymentGateway.Models.Payments
{
    public record GetPaymentsRequest
    {
        public int Top { get; init; }
        public int Skip { get; init; }
        public Guid? ShopperId { get; init; }
        public string OrderBy { get; init; }
        public bool OrderByDescending { get; init; }
    }
}