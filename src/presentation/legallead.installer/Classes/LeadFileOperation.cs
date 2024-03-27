using legallead.installer.Interfaces;
using System.IO.Compression;

namespace legallead.installer.Classes
{
    public class LeadFileOperation : ILeadFileOperation
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void Extract(string path, byte[] content)
        {
            using var ms = new MemoryStream(content);
            using var archive = new ZipArchive(ms);
            archive.ExtractToDirectory(path, true);
        }
    }
}
