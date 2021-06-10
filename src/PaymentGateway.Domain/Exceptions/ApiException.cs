using System;
using System.Net;

namespace PaymentGateway.Domain.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string ErrorMessage { get; }

        public string ErrorCode { get; }
        public ApiException(HttpStatusCode statusCode, string errorMessage, string errorCode, Exception innerException = null)
            :base(errorMessage, innerException)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}