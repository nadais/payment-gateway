namespace PaymentGateway.Application.Common.Abstractions
{
    public interface ICardEncryptionService
    {
        string GetMaskedCardNumber(string cardNumber);
        string GetEncryptedCvv(int cvv);
    }
}