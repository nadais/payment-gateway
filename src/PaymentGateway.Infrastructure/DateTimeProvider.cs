using System;
using PaymentGateway.Application.Common.Abstractions;

namespace PaymentGateway.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentTime() => DateTime.UtcNow;
    }
}