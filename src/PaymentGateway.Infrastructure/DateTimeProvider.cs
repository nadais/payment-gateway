using System;
using PaymentGateway.Application.Abstractions;

namespace TemplateApp.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentTime() => DateTime.UtcNow;
    }
}