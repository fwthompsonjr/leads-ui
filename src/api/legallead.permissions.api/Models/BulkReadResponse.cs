namespace legallead.permissions.api.Models
{
    public class BulkReadResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string OfflineRequestId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = [];
        public int RecordCount { get; set; }
        public int TotalProcessed { get; set; }
    }
}
