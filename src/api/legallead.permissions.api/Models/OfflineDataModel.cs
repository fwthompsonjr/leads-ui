namespace legallead.permissions.api.Models
{
    public class OfflineDataModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string Workload { get; set; } = string.Empty;
        public string Cookie { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string OfflineId { get; set; } = string.Empty;
        public int RowCount { get; set; }
        public int RetryCount { get; set; }
    }
}
