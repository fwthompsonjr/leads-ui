namespace legallead.permissions.api.Models
{
    public class BulkReadRequest
    {
        public string Cookies { get; set; } = string.Empty;
        public string Workload { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
