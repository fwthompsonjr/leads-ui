
namespace legallead.installer.Models
{
    public class ReleaseModel
    {
        public long Id { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReleaseAssetModel> Assets { get; set; } = [];
        public DateTime PublishDate { get; internal set; }
        public string RepositoryName { get; set; } = string.Empty;
    }
}
