using legallead.installer.Interfaces;
using legallead.installer.Models;

namespace legallead.installer.Classes
{
    public class LeadAppInstaller : ILeadAppInstaller
    {
        private const string _subDir = "_ll-applications";
        private readonly string _homeDir;
        private readonly string _applicationsDir;
        private readonly ILeadFileOperation _fileManager;
        public LeadAppInstaller(ILeadFileOperation fileManager)
        {
            _homeDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _applicationsDir = Path.Combine(_homeDir, _subDir);
            _fileManager = fileManager;
        }
        public virtual ILeadFileOperation FileManager => _fileManager;
        public virtual string ParentFolder => _homeDir;

        public virtual string SubFolder => _applicationsDir;

        public string? Install(ReleaseAssetModel model, object? data)
        {
            if (data is not byte[] content) return null;
            if (string.IsNullOrWhiteSpace(model.Name)) return null;
            if (string.IsNullOrWhiteSpace(model.Version)) return null;
            try
            {
                if (!FileManager.DirectoryExists(SubFolder)) FileManager.CreateDirectory(SubFolder);
                var targetDir = Path.Combine(SubFolder, model.Name);
                if (!FileManager.DirectoryExists(targetDir)) FileManager.CreateDirectory(targetDir);
                var installDir = Path.Combine(targetDir, model.Version);
                if (FileManager.DirectoryExists(installDir))
                {
                    // if this version already exists, delete all
                    FileManager.DeleteDirectory(installDir, true);
                }
                FileManager.Extract(installDir, content);
                if (!FileManager.DirectoryExists(installDir)) return string.Empty;
                return installDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
