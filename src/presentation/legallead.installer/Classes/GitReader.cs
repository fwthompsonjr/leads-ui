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
    }
}
