using git.project.reader.models;

namespace git.project.reader.interfaces
{
    public interface IReleaseProvider
    {
        Task<List<AssetModel>> ListAssets(long releaseId);
        Task<List<ReleaseModel>> ListReleases();
    }
}
