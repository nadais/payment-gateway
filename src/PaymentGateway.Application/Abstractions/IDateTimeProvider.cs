using System;

namespace PaymentGateway.Application.Abstractions
{
    public interface IDateTimeProvider
    {
        public DateTime GetCurrentTime();
    }
}