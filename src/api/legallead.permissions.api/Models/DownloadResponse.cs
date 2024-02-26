namespace legallead.permissions.api.Models
{
    public class DownloadResponse
    {
        public string? ExternalId { get; set; }
        public string? Description { get; set; }
        public byte[]? Content { get; set; }
        public string? Error { get; set; }
    }
}
