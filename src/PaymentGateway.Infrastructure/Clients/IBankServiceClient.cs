using System.Threading.Tasks;
using PaymentGateway.Domain.Bank;
using Refit;

namespace PaymentGateway.Infrastructure.Clients
{
    public interface IBankServiceClient
    {
        [Post("/transfer")]
        Task<BankPaymentResponse> PerformCardPayment([Body] BankPaymentRequest payment);
    }
}