namespace PaymentGateway.Application.Common.Abstractions
{
    public interface IEncryptionService
    {
        string Encrypt(string data);
        string Decrypt(string cipherText);
    }
}