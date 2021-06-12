using System.Collections.Generic;

namespace PaymentGateway.Models.Cards
{
    public record CardValidationResponse
    {
        public bool IsValid { get; init; }
        
        public Dictionary<string,ICollection<string>> Errors { get; init; }
    }
}