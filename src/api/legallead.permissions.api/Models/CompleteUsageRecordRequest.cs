namespace legallead.permissions.api.Models
{
    public class CompleteUsageRecordRequest
    {
        public string UsageRecordId { get; set; } = string.Empty;
        public int RecordCount { get; set; }
    }
}