namespace legallead.jdbc.models
{
    public class OfflineDownloadModel
    {
        public string Id { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public bool CanDownload { get; set; }
        public string Workload { get; set; } = string.Empty;
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public string? CourtType { get; set; }
        public int? ItemCount { get; set; }
    }
}
