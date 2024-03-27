using legallead.installer.Interfaces;
using legallead.installer.Models;

namespace legallead.installer.Classes
{
    public class GitReader : IGitReader
    {

        public async Task<List<ReleaseModel>?> GetReleases()
        {
            var releases = await GitClientProvider.GetReleases();
            return releases;
        }
        public async Task<object?> GetAsset(ReleaseAssetModel model)
        {
            var data = await GitClientProvider.GetAsset(model);
            return data;
        }
    }
}
