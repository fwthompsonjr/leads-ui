namespace git.project.reader.models
{
    public class AssetModel
    {
        public long ReleaseId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}
