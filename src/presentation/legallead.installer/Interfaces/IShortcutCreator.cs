using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface IShortcutCreator
    {
        void Build(ReleaseAssetModel model, string targetDir, FileInfo executableFile);
    }
}
