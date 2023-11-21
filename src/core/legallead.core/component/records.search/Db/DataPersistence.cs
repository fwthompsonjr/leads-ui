using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace legallead.records.search.Db
{
    public static class DataPersistence
    {
        internal static string? _appFolder;
        internal static string AppFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_appFolder)) return _appFolder;
                _appFolder = GetAppFolderName();
                return _appFolder;
            }
        }

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppFolderName()
        {
            string execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            return Path.GetDirectoryName(execName) ?? string.Empty;
        }

        private static string? _dataFolder;

        /// <summary>
        /// Gets the data folder.
        /// </summary>
        /// <value>
        /// The data folder.
        /// </value>
        public static string DataFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_dataFolder)) return _dataFolder;
                _dataFolder = GetDataFolderName();
                return _dataFolder;
            }
        }

        /// <summary>
        /// Gets the name of the application data directory.
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        private static string GetDataFolderName()
        {
            string parentName = AppFolder;
            string dataFolder = Path.Combine(parentName, "_db", "_downloads");
            if (Directory.Exists(dataFolder))
            {
                return dataFolder;
            }

            Directory.CreateDirectory(dataFolder);
            return dataFolder;
        }

        public static T GetContent<T>(string fileName) where T : class, new()
        {
            if (!FileExists(fileName))
            {
                return new();
            }
            string targetFile = Path.Combine(DataFolder, fileName);
            using StreamReader reader = new(targetFile);
            string content = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(content) ?? new();
        }

        public static void Save(string fileName, object? data)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string targetFile = Path.Combine(DataFolder, fileName);
            if (File.Exists(targetFile))
            {
                throw new ArgumentOutOfRangeException(nameof(fileName));
            }

            string content = JsonConvert.SerializeObject(data);
            using StreamWriter writer = new(targetFile);
            writer.Write(content);
            writer.Close();
        }

        public static bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string targetFile = Path.Combine(DataFolder, fileName);
            return File.Exists(targetFile);
        }
    }
}