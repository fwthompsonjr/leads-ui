using git.project.reader.models;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IDownloadContentService
    {
        string GetContent(string page, ReleaseModel? model = null);
        Task<DownloadContentResponse> GetDownloadsAsync(string page);
        Task<byte[]?> GetAssetsAsync(long releaseId, string assetName);
        ReleaseAssetBo? GetAssetDetail(long releaseId, long assetId);
    }
}
