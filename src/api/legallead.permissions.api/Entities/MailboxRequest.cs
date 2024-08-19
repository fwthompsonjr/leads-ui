namespace legallead.permissions.api.Entities
{
    public class MailboxRequest
    {
        public string RequestType { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public DateTime? LastUpdate { get; set; }
    }
}
