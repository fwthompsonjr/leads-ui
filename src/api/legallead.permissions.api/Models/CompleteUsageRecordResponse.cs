// CompleteUsageRecordResponse
namespace legallead.permissions.api.Models
{
    public class CompleteUsageRecordResponse
    {
        public string Id { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}