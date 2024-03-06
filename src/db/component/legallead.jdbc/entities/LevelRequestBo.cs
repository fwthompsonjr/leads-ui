namespace legallead.jdbc.entities
{
    public class LevelRequestBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? InvoiceUri { get; set; }
        public string? LevelName { get; set; }
        public string? SessionId { get; set; }
        public bool? IsPaymentSuccess { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
