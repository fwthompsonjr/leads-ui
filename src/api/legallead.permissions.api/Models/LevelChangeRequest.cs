namespace legallead.permissions.api.Models
{
    public class LevelChangeRequest
    {
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? LevelName { get; set; }
        public string? InvoiceUri { get; set; }
        public string? SessionId { get; set; }
    }
}
