namespace legallead.permissions.api.Entities
{
    public class QueueCompletionRequest
    {
        public string UniqueId { get; set; } = string.Empty;
        public string QueryParameter { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
