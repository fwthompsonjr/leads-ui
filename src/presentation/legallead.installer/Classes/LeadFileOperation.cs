using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics;
using System.IO.Compression;

namespace legallead.installer.Classes
{
    public class LeadFileOperation : ILeadFileOperation
    {
        public void CreateDirectory(string path)
        {
            if (DirectoryExists(path)) { return; }
            Directory.CreateDirectory(path);
        }

        public virtual void DeleteDirectory(string path, bool recursive)
        {
            if (!DirectoryExists(path)) { return; }
            Directory.Delete(path, recursive);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Deletes file in the specified path
        /// </summary>
        /// <param name="path">The file location.</param>
        public void DeleteFile(string path)
        {
            if (File.Exists(path)) { File.Delete(path); }
        }
        public bool Extract(string path, byte[] content)
        {
            try
            {
                DeleteDirectory(path, true);
                var progress = new Progress<ZipProgress>();
                progress.ProgressChanged += Report;
                using var ms = new MemoryStream(content);
                using var archive = new ZipArchive(ms);
                archive.ExtractToDirectory(path, progress);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public DirectoryInfoModel[] GetDirectories(string path)
        {
            if (!Directory.Exists(path)) { return []; }
            var found = new DirectoryInfo(path).GetDirectories().Where(d => !d.FullName.EndsWith("backup"));
            if (!found.Any()) { return []; }
            var models = found.Select(s => new DirectoryInfoModel
            {
                Name = s.Name,
                FullName = s.FullName,
                CreateDate = s.CreationTime
            });
            return models.ToArray();
        }

        public virtual IEnumerable<string> FindFiles(string path, string pattern)
        {
            if (!Directory.Exists(path)) { return Enumerable.Empty<string>(); }
            var di = new DirectoryInfo(path);
            return di.GetFiles(pattern, SearchOption.AllDirectories).Select(f => f.FullName);
        }
        public string? FindExecutable(string path)
        {
            if (!DirectoryExists(path)) { return null; }
            var packages = SettingProvider.Common().Packages;
            var files = FindFiles(path, "*.exe").Where(w => IsPackage(w, packages));
            return files.FirstOrDefault();
        }

        public virtual bool IsPackage(string w, List<string> packages)
        {
            const char dash = '-';
            var name = Path.GetFileNameWithoutExtension(w);
            if (name.Contains(dash)) name = name.Split('-')[0];
            return packages.Exists(p => p.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool LaunchExecutable(string path)
        {
            try
            {
                if (!FileExists(path)) { return false; }
                Process myProcess = new();
                var info = myProcess.StartInfo;
                info.WorkingDirectory = Path.GetDirectoryName(path);
                info.WindowStyle = ProcessWindowStyle.Normal;
                info.FileName = path;
                info.CreateNoWindow = false;
                info.UseShellExecute = true;
                myProcess.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void Report(object? sender, ZipProgress zipProgress)
        {
            zipProgress.Echo();
        }
    }
}
