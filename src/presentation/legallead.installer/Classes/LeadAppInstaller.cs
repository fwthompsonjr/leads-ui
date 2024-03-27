using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.IO.Compression;

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

        public virtual string ParentFolder => _homeDir;

        public virtual string SubFolder => _applicationsDir;

        public string? Install(ReleaseAssetModel model, object? data)
        {
            if (data is not byte[] content) return null;
            try
            {
                var targetDir = Path.Combine(SubFolder, model.Name);
                if (!_fileManager.DirectoryExists(targetDir)) _fileManager.CreateDirectory(targetDir);
                var installDir = Path.Combine(targetDir, model.Version);
                if (_fileManager.DirectoryExists(installDir))
                {
                    // if this version already exists, delete all
                    _fileManager.DeleteDirectory(installDir, true);
                }
                _fileManager.Extract(installDir, content);
                if (!_fileManager.DirectoryExists(installDir)) return string.Empty;
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
