namespace legallead.installer.Classes
{
    internal static class EnvironmentStorageKey
    {
        internal static readonly Dictionary<string, string> StorageKeys = new()
        {
            { "Git.Releases", "LEGALLEAD_INS_GIT_RELEASES" },
            { "Git.Assets", "LEGALLEAD_INS_GIT_RELEASE_ASSETS" },
            { "Git.Repository", "LEGALLEAD_INS_GIT_REPOSITORY" },
        };
    }
}
