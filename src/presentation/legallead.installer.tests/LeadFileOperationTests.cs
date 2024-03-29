using Bogus;
using legallead.installer.Classes;
using System.IO.Compression;
using System.Reflection;

namespace legallead.installer.tests
{
    public class LeadFileOperationTests
    {
        private static readonly object locker = new();
        public LeadFileOperationTests()
        {
            _ = CreateTempExtract();
        }

        [Fact]
        public void TempExtractIsCreated()
        {
            var filePath = CreateTempExtract();
            Assert.False(string.IsNullOrEmpty(filePath));
        }

        [Fact]
        public void OpsCanCreateDirectory()
        {
            var sut = new LeadFileOperation();
            var filePath = CreateTempExtract();
            Assert.False(string.IsNullOrEmpty(filePath));
            var testDir = Path.GetDirectoryName(filePath);
            Assert.False(string.IsNullOrEmpty(testDir));
            var subDirName = Path.Combine(testDir, Path.GetFileName(new Faker().System.DirectoryPath()));
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    sut.CreateDirectory(subDirName);
                    Assert.True(Directory.Exists(subDirName)); 
                }
            }
            finally
            {
                if (Directory.Exists(subDirName)) Directory.Delete(subDirName);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpsCanDeleteDirectory(bool isRecursive)
        {
            var sut = new LeadFileOperation();
            var filePath = CreateTempExtract();
            Assert.False(string.IsNullOrEmpty(filePath));
            var testDir = Path.GetDirectoryName(filePath);
            Assert.False(string.IsNullOrEmpty(testDir));
            var subDirName = Path.Combine(testDir, Path.GetFileName(new Faker().System.DirectoryPath()));
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0) sut.CreateDirectory(subDirName);
                    sut.DeleteDirectory(subDirName, isRecursive);
                    Assert.False(Directory.Exists(subDirName));
                }
            }
            finally
            {
                if (Directory.Exists(subDirName)) Directory.Delete(subDirName);
            }
        }

        [Fact]
        public void OpsCanExtractToDirectory()
        {
            var sut = new LeadFileOperation();
            string extractDir = Path.Combine(CurrentDir, "_test_extract");
            lock (locker)
            {
                try
                {
                    var filePath = CreateTempExtract();
                    Assert.True(File.Exists(filePath));
                    var content = File.ReadAllBytes(filePath);
                    sut.Extract(extractDir, content);
                    Assert.True(Directory.Exists(extractDir));
                }
                finally
                {
                    if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                } 
            }

        }

        private static string? CreateTempExtract()
        {
            try
            {
                lock (locker)
                {
                    var currentDir = CurrentDir;
                    if (!Directory.Exists(currentDir)) { return null; }
                    var sampleFolder = Path.Combine(currentDir, "sample");
                    if (!Directory.Exists(sampleFolder)) { return null; }
                    string tempPath = Path.Combine(currentDir, "_test_results");
                    if (!Directory.Exists(tempPath)) { Directory.CreateDirectory(tempPath); }
                    string zipPath = Path.Combine(tempPath, "sample.zip");
                    if (File.Exists(zipPath)) return zipPath;
                    ZipFile.CreateFromDirectory(sampleFolder, zipPath);
                    if (File.Exists(zipPath)) return zipPath;
                    return null; 
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static string? _currentDir;
        private static string CurrentDir
        {
            get
            {
                if (_currentDir != null) return _currentDir;
                _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return _currentDir ?? string.Empty;
            }
        }
    }
}
