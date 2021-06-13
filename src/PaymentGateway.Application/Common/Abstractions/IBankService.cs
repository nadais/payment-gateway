using System.Threading.Tasks;
using PaymentGateway.Domain.Bank;

namespace PaymentGateway.Application.Common.Abstractions
{
    public interface IBankService
    {
        Task<BankPaymentResponse> ProcessCardPaymentAsync(BankPaymentRequest request);
    }
}