namespace legallead.installer.Models
{
    internal class ReleaseAssetModel
    {
        public long AssetId { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;

    }
}
