using Bogus;
using legallead.installer.Classes;
using System.Diagnostics;
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
        [InlineData(null, "*.txt")]
        [InlineData(true, "*.txt")]
        [InlineData(false, "*.xls")]
        public void OpsCanFindFiles(bool? expected, string pattern)
        {
            var sut = new LeadFileOperation();
            var currentDir = CurrentDir;
            var sampleFolder = expected.HasValue ? Path.Combine(currentDir, "sample") : "Not-A-Valid-Path";
            var files = sut.FindFiles(sampleFolder, pattern);
            Assert.NotNull(files);
            if (expected.GetValueOrDefault()) { Assert.NotEmpty(files); }
            else { Assert.Empty(files); }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public void OpsCanGetDirectories(bool? expected)
        {
            var sut = new LeadFileOperation();
            var currentDir = CurrentDir;
            var sampleFolder = expected.GetValueOrDefault() ? currentDir : "Not-A-Valid-Path";
            var folders = sut.GetDirectories(sampleFolder);
            if (expected.GetValueOrDefault()) { Assert.NotEmpty(folders); }
            else { Assert.Empty(folders); }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public void OpsCanFindExecutable(bool? expected)
        {
            var sut = new LeadFileOperation();
            var currentDir = CurrentDir;
            var sampleFolder = expected.GetValueOrDefault() ? currentDir : "Not-A-Valid-Path";
            var exeFile = sut.FindExecutable(sampleFolder);
            if (expected.GetValueOrDefault()) { Assert.False(string.IsNullOrEmpty(exeFile)); }
            else { Assert.Null(exeFile); }
        }

        [Fact]
        public void OpsCanLaunchExecutable()
        {
            var sut = new LeadFileOperation();
            var currentDir = CurrentDir;
            var exeFile = sut.FindExecutable(currentDir);
            Assert.False(string.IsNullOrEmpty(exeFile));
            Assert.True(sut.LaunchExecutable(exeFile));
        }


        [Fact]
        public void OpsCanNotLaunchExecutableForMissingFile()
        {
            var sut = new LeadFileOperation();
            var currentDir = CurrentDir;
            var exeFile = sut.FindExecutable(currentDir);
            Assert.False(string.IsNullOrEmpty(exeFile));
            exeFile = exeFile.Replace(".", "-");
            Assert.False(sut.LaunchExecutable(exeFile));
        }

        [Fact]
        public void OpsCanNotLaunchExecutableWhenExceptionIsThrown()
        {
            var sut = new MockLeadFileOperation();
            var currentDir = CurrentDir;
            var exeFile = sut.FindExecutable(currentDir);
            Assert.False(string.IsNullOrEmpty(exeFile));
            Assert.False(sut.LaunchExecutable(exeFile));
        }

        [Fact]
        public void OpsCanLaunchNotepad()
        {
            var notepad = Environment.SystemDirectory + "\\notepad.exe";
            if (!File.Exists(notepad)) { return; }
            try
            {
                var sut = new LeadFileOperation();
                var opened = sut.LaunchExecutable(notepad);
                Assert.True(opened);
            }
            finally
            {
                var proc = Process.GetProcessesByName("notepad")?.ToList();
                proc?.ForEach(p => p.Kill());
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
                    Assert.True(sut.FileExists(filePath));
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

        [Fact]
        public void OpsExtractToDirectoryHandlesExceptions()
        {
            var sut = new MockLeadFileOperation();
            string extractDir = Path.Combine(CurrentDir, "_test_extract");
            lock (locker)
            {
                try
                {
                    var filePath = CreateTempExtract();
                    var content = File.ReadAllBytes(filePath);
                    var response = sut.Extract(extractDir, content);
                    Assert.False(response);
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

        private sealed class MockLeadFileOperation : LeadFileOperation
        {
            public override bool FileExists(string path)
            {
                var exception = new Faker().System.Exception();
                throw exception;
            }
            public override void DeleteDirectory(string path, bool recursive)
            {
                var exception = new Faker().System.Exception();
                throw exception;
            }
        }
    }
}
