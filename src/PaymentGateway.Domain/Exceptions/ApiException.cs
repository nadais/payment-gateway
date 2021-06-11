using System;
using System.Collections.Generic;
using System.Net;

namespace PaymentGateway.Domain.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string ErrorMessage { get; }

        public string ErrorCode { get; }

        public Dictionary<string,ICollection<string>> ErrorInformation { get; set; }

        public ApiException(HttpStatusCode statusCode, string errorMessage, string errorCode, Exception innerException = null, Dictionary<string,ICollection<string>> errorInformation = null)
            :base(errorMessage, innerException)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ErrorInformation = errorInformation;
        }
    }
}