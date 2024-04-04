namespace legallead.installer.Models
{
    public class LocalVersionModel
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime PublishDate { get; internal set; }
        public string FullPath { get; internal set; }
    }
}
