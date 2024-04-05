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
            var found = new DirectoryInfo(path).GetDirectories();
            if (found == null || found.Length == 0) { return []; }
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
            return FindFiles(path, "*.exe").FirstOrDefault();
        }

        public bool LaunchExecutable(string path)
        {
            try
            {
                if (!FileExists(path)) { return false; }
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(path) { 
                        WindowStyle = ProcessWindowStyle.Normal, 
                        CreateNoWindow = false },
                };
                process.Start();
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
