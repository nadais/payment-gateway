using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Bank;

namespace PaymentGateway.Infrastructure.Services
{
    public class BankService : IBankService
    {
        public bool ProcessCardPayment(BankPaymentRequest request)
        {
            return request.Quantity > 0;
        }
    }
}