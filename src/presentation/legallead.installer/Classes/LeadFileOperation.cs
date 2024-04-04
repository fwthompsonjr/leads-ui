using legallead.installer.Interfaces;
using legallead.installer.Models;
using System;
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

        public void DeleteDirectory(string path, bool recursive)
        {
            if (!DirectoryExists(path)) { return; }
            Directory.Delete(path, recursive);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
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

        private void Report(object? sender, ZipProgress zipProgress)
        {
            zipProgress.Echo();
        }
    }
}
