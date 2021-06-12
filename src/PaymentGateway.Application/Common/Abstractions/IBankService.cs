using PaymentGateway.Domain.Bank;

namespace PaymentGateway.Application.Common.Abstractions
{
    public interface IBankService
    {
        bool ProcessCardPayment(BankPaymentRequest request);
    }
}