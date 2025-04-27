using git.project.reader.models;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IDownloadContentService
    {
        string GetContent(string page, ReleaseModel? model = null);
        Task<DownloadContentResponse> GetDownloadsAsync(string page);
        Task<string> GetAssetsAsync(string page);
    }
}
