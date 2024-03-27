using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    internal interface ILeadAppInstaller
    {
        string ParentFolder { get; }
        string SubFolder { get; }
        string? Install(ReleaseAssetModel model, object? data);
    }
}
