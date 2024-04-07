using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface ILeadAppInstaller
    {
        string ParentFolder { get; }
        string SubFolder { get; }
        ILeadFileOperation FileManager { get; }

        List<LocalAppModel> GetInstalledApplications();
        string? Install(ReleaseAssetModel model, object? data);
    }
}
