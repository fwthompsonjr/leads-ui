using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface IShortcutCreator
    {
        string? Build(ReleaseAssetModel model, string targetDir, FileInfo executableFile);
        void Install(IShortcutCreator service, ReleaseAssetModel item, string installPath, string name, string version);
    }
}
