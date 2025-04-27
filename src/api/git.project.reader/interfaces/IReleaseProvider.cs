using git.project.reader.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace git.project.reader.interfaces
{
    public interface IReleaseProvider
    {
        Task<List<AssetModel>> ListAssets(long releaseId);
        Task<List<ReleaseModel>> ListReleases();
    }
}
