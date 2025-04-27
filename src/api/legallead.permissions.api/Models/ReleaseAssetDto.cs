namespace legallead.permissions.api.Models
{
    public class ReleaseAssetDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
