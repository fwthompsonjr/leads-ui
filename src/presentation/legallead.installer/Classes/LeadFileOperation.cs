using legallead.installer.Interfaces;
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

        private void Report(object? sender, ZipProgress zipProgress)
        {
            zipProgress.Echo();
        }
    }
}
