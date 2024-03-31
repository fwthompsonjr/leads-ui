using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface IGitReader
    {
        bool AllowShortcuts { get; }

        Task<List<ReleaseModel>?> GetReleases();
        Task<List<ReleaseAssetModel>?> GetAssets();
        Task<object?> GetAsset(ReleaseAssetModel model);
        ReleaseAssetModel? FindAsset(List<ReleaseModel> models, string version, string app);
        bool VerifyPackageName(string packageName);
    }
}
