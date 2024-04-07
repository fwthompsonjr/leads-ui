using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

namespace legallead.installer.Classes
{
    internal static class ZipFileExtensions
    {
        public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<ZipProgress> progress)
        {
            ExtractToDirectory(source, destinationDirectoryName, progress, overwrite: false);
        }

        public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<ZipProgress> progress, bool overwrite)
        {

            // Note that this will give us a good DirectoryInfo even if destinationDirectoryName exists:
            DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
            string destinationDirectoryFullPath = di.FullName;

            int count = 0;
            var entries = source.Entries.ToList();
            entries.ForEach(entry =>
            {
                count = entries.IndexOf(entry) + 1;
                var zipProgress = new ZipProgress(source.Entries.Count, count, entry.FullName);
                progress.Report(zipProgress);
                string fileDestinationPath = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, entry.FullName));
                TestPathLocation(fileDestinationPath, destinationDirectoryFullPath);
                Extract(entry, fileDestinationPath, overwrite);
            });
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested thru publicly exposed member")]
        private static void Extract(ZipArchiveEntry entry, string fileDestinationPath, bool overwrite)
        {
            if (Path.GetFileName(fileDestinationPath).Length == 0)
            {
                TryCreateDirectory(entry, fileDestinationPath);
            }
            else
            {
                TryExtractFile(entry, fileDestinationPath, overwrite);
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested thru publicly exposed member")]
        private static void TestPathLocation(string fileDestinationPath, string destinationDirectoryFullPath)
        {
            if (!fileDestinationPath.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                throw new IOException("File is extracting to outside of the folder specified.");
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested thru publicly exposed member")]
        private static void TryCreateDirectory(ZipArchiveEntry entry, string fileDestinationPath)
        {
            if (Path.GetFileName(fileDestinationPath).Length != 0) return;
            if (entry.Length != 0) throw new IOException("Directory entry with data.");
            Directory.CreateDirectory(fileDestinationPath);
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested thru publicly exposed member")]
        private static void TryExtractFile(ZipArchiveEntry entry, string fileDestinationPath, bool overwrite)
        {
            if (Path.GetFileName(fileDestinationPath).Length == 0) return;
            var parentDirectory = Path.GetDirectoryName(fileDestinationPath);
            if (string.IsNullOrEmpty(parentDirectory)) return;
            if (!Directory.Exists(parentDirectory)) Directory.CreateDirectory(parentDirectory);
            entry.ExtractToFile(fileDestinationPath, overwrite: overwrite);
        }
    }
}
