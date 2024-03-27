using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface IGitReader
    {
        Task<List<ReleaseModel>?> GetReleases();
        Task<object?> GetAsset(ReleaseAssetModel model);
    }
}
