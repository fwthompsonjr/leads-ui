using git.project.reader.models;

namespace legallead.permissions.api.Models
{
    public class DownloadContentResponse
    {
        public string Content { get; set; } = string.Empty;
        public List<ReleaseModel> Models { get; set; } = [];
        public DateTime CreationDate { get; set; }
    }
}
