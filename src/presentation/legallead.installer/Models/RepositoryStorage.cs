using legallead.installer.Interfaces;
using Octokit;

namespace legallead.installer.Models
{
    public class RepositoryStorage : ModelStorageBase<Repository>
    {
        public override string Name => "Git.Repository";
    }
}