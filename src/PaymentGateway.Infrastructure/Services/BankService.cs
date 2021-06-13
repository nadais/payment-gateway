using System.Threading.Tasks;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Bank;
using PaymentGateway.Infrastructure.Clients;
using Refit;

namespace PaymentGateway.Infrastructure.Services
{
    public class BankService : IBankService
    {
        private readonly IBankServiceClient _bankServiceClient;

        public BankService(IBankServiceClient bankServiceClient)
        {
            _bankServiceClient = bankServiceClient;
        }

        public async Task<BankPaymentResponse> ProcessCardPaymentAsync(BankPaymentRequest request)
        {
            try
            {
                return await _bankServiceClient.PerformCardPayment(request);
            }
            catch (ApiException refitApiException)
            {
                throw new Domain.Exceptions.ApiException(refitApiException.StatusCode,
                    "An error occured when connecting to bank API"
                    , "BANK_SERVICE_ERROR", refitApiException);
            }
        }
    }
}