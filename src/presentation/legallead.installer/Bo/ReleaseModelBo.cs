using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Bo
{
    [ExcludeFromCodeCoverage(Justification = "Item scheduled for deletion")]
    internal class ReleaseModelBo
    {
        public long Id { get; set; }
        public long RepositoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ReleaseAssetModel> Assets { get; set; } = [];
        public string PublishDate { get; set; } = string.Empty;
    }
}