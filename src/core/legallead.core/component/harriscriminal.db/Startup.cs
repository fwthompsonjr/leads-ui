using legallead.harriscriminal.db.Downloads;
using legallead.harriscriminal.db.Tables;
using System.Globalization;
using System.Reflection;

namespace legallead.harriscriminal.db
{
    public static class Startup
    {
        private static string? _appFolder;
        internal static string AppFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_appFolder)) return _appFolder;
                _appFolder = GetAppFolderName();
                return _appFolder;
            }
        }

        public static string DataFolder => Downloads.DatFolder;

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppFolderName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            return Path.GetDirectoryName(execName) ?? string.Empty;
        }

        public static void Read()
        {
            References.ReadReferences();
            Downloads.ReadFiles();
            CaseStyles.ReadStyles();
        }

        public static Task ReadAsync(IProgress<bool> progress)
        {
            return Task.Run(() =>
            {
                Read();
                progress.Report(true);
            });
        }

        public static void Reset()
        {
            References.ReadReferences(true);
            Downloads.ReadFiles();
        }

        public static class Downloads
        {
            private enum FileDateType
            {
                Max,
                Min
            }

            private static string? _dataFolder;

            internal static string DatFolder
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
            private static string GetDataFolderName()
            {
                var parentName = AppFolder;
                return Path.Combine(parentName, "_db", "_downloads");
            }

            public static List<string> FileNames { get; private set; } = new();

            public static List<HarrisCountyListDto> DataList { get; private set; } = new();

            public static void ReadFiles(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }
                const string extn = "*CrimFilingsWithFutureSettings*.txt";
                var directory = new DirectoryInfo(DatFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                FileNames.Sort((a, b) => b.CompareTo(a));
                var records = new List<HarrisCountyListDto>();
                foreach (var item in files)
                {
                    var data = HarrisCriminalDto.Map(item.FullName);
                    var business = HarrisCriminalBo.Map(data);
                    business.Sort((a, b) =>
                    {
                        int aa = a.DateFiled.CompareTo(b.DateFiled);
                        if (aa != 0)
                        {
                            return aa;
                        }

                        int bb = a.NextAppearanceDate.CompareTo(b.NextAppearanceDate);
                        return bb;
                    });
                    records.Add(new HarrisCountyListDto
                    {
                        CreateDate = item.CreationTime,
                        FileDate = GetFileDate(item.CreationTime, data),
                        MaxFilingDate = GetMinOrMax(DateTime.MinValue, data, FileDateType.Max),
                        MinFilingDate = GetMinOrMax(DateTime.MinValue, data, FileDateType.Min),
                        Name = Path.GetFileNameWithoutExtension(item.Name),
                        Data = data,
                        BusinessData = business
                    });
                }
                DataList = records;
            }

            private static DateTime GetMinOrMax(DateTime minValue, List<HarrisCriminalDto> data, FileDateType dateType)
            {
                if (data == null || !data.Any())
                {
                    return minValue;
                }
                DateTime lookupDate = minValue;
                switch (dateType)
                {
                    case FileDateType.Max:
                        lookupDate = data.Max(m =>
                        m.FilingDate.ToExactDate("yyyyMMdd", minValue));
                        break;

                    case FileDateType.Min:
                        lookupDate = data.Min(m =>
                        m.FilingDate.ToExactDate("yyyyMMdd", DateTime.MaxValue));
                        if (lookupDate.Equals(DateTime.MaxValue))
                        {
                            lookupDate = minValue;
                        }
                        break;
                }
                return lookupDate;
            }

            private static DateTime GetFileDate(DateTime creationTime, List<HarrisCriminalDto> data)
            {
                if (data == null || !data.Any())
                {
                    return creationTime;
                }
                var datum = data.FirstOrDefault();
                if (datum == null)
                {
                    return creationTime;
                }
                if (DateTime.TryParse(datum.DateDatasetProduced, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime dataDate))
                {
                    return dataDate;
                }
                return creationTime;
            }
        }

        public static class References
        {
            private static string? _dataFolder;

            private static string DtFolder => _dataFolder ??= GetDataFolderName();

            /// <summary>
            /// Gets the name of the application data directory.
            /// </summary>
            /// <returns></returns>
            private static string GetDataFolderName()
            {
                var parentName = AppFolder;
                return Path.Combine(parentName, "_db", "_tables");
            }

            /// <summary>
            /// List of full filenames found
            /// </summary>
            public static List<string> FileNames { get; private set; } = new();

            /// <summary>
            /// List of Reference Data
            /// </summary>
            public static List<ReferenceTable> DataList { get; private set; } = new();

            /// <summary>
            /// Reads Reference Data and Stores in Memory
            /// </summary>
            /// <param name="reset"></param>
            public static void ReadReferences(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }
                const string extn = "*hcc.tables.*.json";
                var directory = new DirectoryInfo(DtFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                var tables = new List<ReferenceTable>();
                FileNames.ForEach(f => { tables.Add(GenericRead<ReferenceTable>(f)); });
                DataList = tables;
            }

            private static T GenericRead<T>(string sourceFileName) where T : class, new()
            {
                var content = GetFileContent(sourceFileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content) ?? new();
            }

            private static string GetFileContent(string sourceFileName)
            {
                using var reader = new StreamReader(sourceFileName);
                return reader.ReadToEnd();
            }
        }

        public static class CaseStyles
        {
            private static string? _dataFolder;

            private static string DaFolder => _dataFolder ??= GetDataFolderName();

            /// <summary>
            /// Gets the name of the application data directory.
            /// </summary>
            /// <returns></returns>
            private static string GetDataFolderName()
            {
                var parentName = AppFolder;
                return Path.Combine(parentName, "_db", "_downloads");
            }

            /// <summary>
            /// List of full filenames found
            /// </summary>
            public static List<string> FileNames { get; private set; } = new();

            /// <summary>
            /// List of Reference Data
            /// </summary>
            public static List<CaseStyleDb> DataList { get; private set; } = new();

            /// <summary>
            /// Reads Reference Data and Stores in Memory
            /// </summary>
            /// <param name="reset"></param>
            public static void ReadStyles(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }

                const string extn = "*HarrisCriminalStyleDto.json";
                var directory = new DirectoryInfo(DaFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                FileNames.Sort((a, b) => b.CompareTo(a));
                var tables = new List<CaseStyleDb>();
                FileNames.ForEach(f => { tables.AddRange(FileRead<List<CaseStyleDb>>(f)); });
                DataList = tables;
            }

            private static T FileRead<T>(string sourceFileName) where T : class, new()
            {
                var content = GetFileContent(sourceFileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content) ?? new();
            }

            private static string GetFileContent(string sourceFileName)
            {
                using var reader = new StreamReader(sourceFileName);
                return reader.ReadToEnd();
            }
        }
    }
}