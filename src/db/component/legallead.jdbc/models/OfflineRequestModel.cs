namespace legallead.jdbc.models
{
    public class OfflineRequestModel
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
