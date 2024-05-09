using legallead.installer.Bo;
using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    internal static class ModelStorageMapper
    {
        internal static readonly Dictionary<Type, Type> Translators = new()
        {
            { typeof(ReleaseModelStorage), typeof(ReleaseModelStorageBo) },
            { typeof(RepositoryStorage), typeof(RepositoryStorageBo) },
        };
    }
}
