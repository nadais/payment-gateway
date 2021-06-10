namespace PaymentGateway.Models
{
    public record ErrorResponse
    {
        public string TraceIdentifier { get; set; }
        public string ErrorCode { get; init; }

        public string Message { get; init; }
    }
}