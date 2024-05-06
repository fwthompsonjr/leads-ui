using Xunit.Abstractions;

namespace permissions.api.tests
{
    public class ApiIntegrationComplianceTests
    {
        private readonly ITestOutputHelper output;

        public ApiIntegrationComplianceTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void PostmanSummaryShouldExist()
        {
            var postmanFile = Locator.PostmanSummaryFile();
            Assert.False(string.IsNullOrEmpty(postmanFile));
            Assert.True(File.Exists(postmanFile));
        }

        [Fact]
        public void PostmanSummaryShouldBeCurrent()
        {
            var latestApplicationFile = Locator.GetCsFiles().FirstOrDefault();
            var latestWriteDate = latestApplicationFile?.CreationTime;
            var postmanFile = Locator.PostmanSummaryFile();
            Assert.NotNull(latestApplicationFile);
            Assert.True(File.Exists(postmanFile));
            output.WriteLine("Last application file is: {0}, {1:s}", 
                latestApplicationFile.Name, 
                latestWriteDate.GetValueOrDefault());
            var postmanDate = new FileInfo(postmanFile).CreationTime;

            output.WriteLine("Last intergration file is: {0}, {1:s}",
                Path.GetFileNameWithoutExtension(postmanFile),
                postmanDate);
            Assert.True(postmanDate > latestWriteDate.GetValueOrDefault());
        }

        [Fact]
        public void ApplicationFolderShouldExist()
        {
            var appFolder = Locator.ApplicationFolder();
            Assert.False(string.IsNullOrEmpty(appFolder));
            Assert.True(Directory.Exists(appFolder));
        }

        [Fact]
        public void ApplicationFolderShouldContainCsFiles()
        {
            var appFiles = Locator.GetCsFiles();
            Assert.NotEmpty(appFiles);
        }

        private static class Locator
        {
            private static readonly object locker = new();
            static Locator()
            {
                lock (locker)
                {
                    _ = ExecutableFile();
                    _ = ExecutableFolder();
                    _ = LeadsFolder();
                    _ = PostmanFolder();
                    _ = PostmanSummaryFile();
                    _ = ApplicationFolder();
                }
            }
            private static string? _exeFile;
            private static string? _exeFolder;
            private static string? _applicationFolder;
            private static string? _leadsFolder;
            private static string? _postmanFolder;
            private static string? _postmanSummaryFile;

            public static string ExecutableFile()
            {
                if (!string.IsNullOrEmpty(_exeFile)) return _exeFile;
                var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return string.Empty;
                _exeFile = path;
                return _exeFile;
            }
            public static string ExecutableFolder()
            {
                if (!string.IsNullOrEmpty(_exeFolder)) return _exeFolder;
                var path = ExecutableFile();
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return string.Empty;
                var parent = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(parent) || !Directory.Exists(parent)) return string.Empty;
                _exeFolder = parent;
                return _exeFolder;
            }

            public static string LeadsFolder()
            {
                const string rootName = "leads-ui";
                if (!string.IsNullOrEmpty(_leadsFolder)) return _leadsFolder;
                var path = ExecutableFolder();
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return string.Empty;
                var parentDir = Path.GetDirectoryName(path);
                var parentName = Path.GetFileName(parentDir);
                var indx = 0;
                while (!string.IsNullOrEmpty(parentName) &&
                    !parentName.Equals(rootName, StringComparison.OrdinalIgnoreCase) &&
                    indx < 10)
                {
                    parentDir = Path.GetDirectoryName(parentDir);
                    parentName = Path.GetFileName(parentDir);
                    indx++;
                }
                if (string.IsNullOrEmpty(parentDir) || !Directory.Exists(parentDir)) return string.Empty;
                _leadsFolder = parentDir;
                return _leadsFolder;
            }

            public static string PostmanFolder()
            {
                const string rootName = "postman";
                if (!string.IsNullOrEmpty(_postmanFolder)) return _postmanFolder;
                var path = LeadsFolder();
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return string.Empty;
                var parentDir = Path.Combine(path, rootName);
                if (string.IsNullOrEmpty(parentDir) || !Directory.Exists(parentDir)) return string.Empty;
                _postmanFolder = parentDir;
                return _postmanFolder;
            }

            public static string PostmanSummaryFile()
            {
                const string rootName = "ll-authorizations-summary.txt";
                if (!string.IsNullOrEmpty(_postmanSummaryFile)) return _postmanSummaryFile;
                var path = PostmanFolder();
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return string.Empty;
                var textFile = Path.Combine(path, rootName);
                if (string.IsNullOrEmpty(textFile) || !File.Exists(textFile)) return string.Empty;
                _postmanSummaryFile = textFile;
                return _postmanSummaryFile;
            }

            public static string ApplicationFolder()
            {
                const string rootName = "src/api/legallead.permissions.api";
                if (!string.IsNullOrEmpty(_applicationFolder)) return _applicationFolder;
                var path = LeadsFolder();
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return string.Empty;
                var parentDir = Path.Combine(path, rootName);
                if (string.IsNullOrEmpty(parentDir) || !Directory.Exists(parentDir)) return string.Empty;
                _applicationFolder = parentDir;
                return _applicationFolder;
            }

            public static List<FileInfo> GetCsFiles()
            {
                var directory = ApplicationFolder();
                var files = new DirectoryInfo(directory)
                    .GetFiles("*.cs", SearchOption.AllDirectories)
                    .Where(f => !f.FullName.Contains("\\bin\\"))
                    .Where(f => !f.FullName.Contains("\\obj\\"))
                    .ToList();

                files.Sort((b,a) => a.CreationTime.CompareTo(b.CreationTime));
                return files;
            }
        }
    }
}
