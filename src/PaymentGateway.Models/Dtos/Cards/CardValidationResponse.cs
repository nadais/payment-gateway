namespace PaymentGateway.Models.Dtos.Cards
{
    public record CardValidationResponse
    {
        public bool IsValid { get; set; }
    }
}