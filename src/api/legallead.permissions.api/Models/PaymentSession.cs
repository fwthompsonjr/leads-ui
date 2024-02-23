namespace legallead.permissions.api.Models
{
    public class PaymentSession
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? InvoiceId { get; set; }
        public string? SessionType { get; set; }
        public string? SessionId { get; set; }
        public string? IntentId { get; set; }
        public string? ClientId { get; set; }
        public string? ExternalId { get; set; }
    }
}
