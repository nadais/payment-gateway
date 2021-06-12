using System;
using System.Net;

namespace PaymentGateway.Domain.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string errorMessage, Exception innerException = null) :
            base(HttpStatusCode.NotFound, errorMessage, "NOT_FOUND", innerException)
        {
        }
    }
}