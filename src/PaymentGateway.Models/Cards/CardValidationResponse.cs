namespace PaymentGateway.Models.Cards
{
    public record CardValidationResponse
    {
        public bool IsValid { get; set; }
    }
}