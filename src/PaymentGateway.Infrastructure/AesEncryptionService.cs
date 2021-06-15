using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Infrastructure.Options;

namespace PaymentGateway.Infrastructure
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly EncryptionOptions _options;

        public AesEncryptionService(IOptions<EncryptionOptions> options)
        {
            _options = options.Value;
        }

        public string Encrypt(string data)
        {
            var iv = new byte[16];
            var key = _options.Key;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(data);
            }

            var array = memoryStream.ToArray();
            return Convert.ToBase64String(array);
        }

        public string Decrypt(string cipherText)
        {
            var key = _options.Key;
            var iv = new byte[16];
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}