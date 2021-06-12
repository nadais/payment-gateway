using System;

namespace PaymentGateway.Application.Common.Abstractions
{
    public interface IDateTimeProvider
    {
        public DateTime GetCurrentTime();
    }
}