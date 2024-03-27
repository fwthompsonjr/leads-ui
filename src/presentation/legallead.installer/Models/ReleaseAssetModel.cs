namespace legallead.installer.Models
{
    public class ReleaseAssetModel
    {
        public int AssetId { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;

    }
}
