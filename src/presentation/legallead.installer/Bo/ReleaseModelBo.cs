using legallead.installer.Models;

namespace legallead.installer.Bo
{
    internal class ReleaseModelBo
    {
        public long Id { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReleaseAssetModel> Assets { get; set; } = [];
        public string PublishDate { get; set; } = string.Empty;
    }
}