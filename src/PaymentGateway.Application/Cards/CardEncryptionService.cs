using System.Security.Cryptography;
using System.Text;
using PaymentGateway.Application.Common.Abstractions;

namespace PaymentGateway.Application.Cards
{
    public class CardEncryptionService : ICardEncryptionService
    {
        public string GetMaskedCardNumber(string cardNumber)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < cardNumber.Length; i++)
            {
                if (i < cardNumber.Length - 4)
                {
                    sb.Append('*');
                }

                sb.Append(cardNumber[i]);
            }

            return sb.ToString();
        }
        public string GetEncryptedCvv(int cvv)
        {
            var bytes = Encoding.UTF8.GetBytes(cvv.ToString());
            var hashString = new SHA256Managed();
            var hash = hashString.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var x in hash)
            {
                sb.Append($"{x:x2}");
            }
            return sb.ToString();
            
        }
    }
}