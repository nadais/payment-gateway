using System;
using System.Collections.Generic;
using System.Net;

namespace PaymentGateway.Domain.Exceptions
{
    public class CardInvalidException : ApiException
    {
        public CardInvalidException(Dictionary<string,ICollection<string>> errors, Exception innerException = null) : base(HttpStatusCode.BadRequest, "The provided information for card is invalid", "INVALID_CARD", innerException, errors)
        {
        }
    }
}