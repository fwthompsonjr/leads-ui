﻿using Harris.Criminal.Db.Downloads;
using Harris.Criminal.Db.Tables;
using System.Globalization;
using System.Reflection;

namespace Harris.Criminal.Db
{
    public static class Startup
    {
        private static string? _appFolder;
        internal static string AppFolder => _appFolder ??= GetAppFolderName();

        public static string DataFolder => Downloads.DataFolder;

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
            References.Read();
            Downloads.Read();
            CaseStyles.Read();
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
            References.Read(true);
            Downloads.Read();
        }

        public static class Downloads
        {
            private enum FileDateType
            {
                Max,
                Min
            }

            private static string? _dataFolder;

            internal static string DataFolder => _dataFolder ??= GetDataFolderName();

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

            public static void Read(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }
                const string extn = "*CrimFilingsWithFutureSettings*.txt";
                var directory = new DirectoryInfo(DataFolder);
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

            private static string DataFolder => _dataFolder ??= GetDataFolderName();

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
            public static void Read(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }
                const string extn = "*hcc.tables.*.json";
                var directory = new DirectoryInfo(DataFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                var tables = new List<ReferenceTable>();
                FileNames.ForEach(f => { tables.Add(Read<ReferenceTable>(f)); });
                DataList = tables;
            }

            private static T Read<T>(string sourceFileName) where T : class, new()
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

            private static string DataFolder => _dataFolder ??= GetDataFolderName();

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
            public static void Read(bool reset = false)
            {
                if (!reset && FileNames != null && DataList != null && FileNames.Count > 0)
                {
                    return;
                }

                const string extn = "*HarrisCriminalStyleDto.json";
                var directory = new DirectoryInfo(DataFolder);
                var files = directory.GetFiles(extn).ToList();
                FileNames = files.Select(f => f.FullName).ToList();
                FileNames.Sort((a, b) => b.CompareTo(a));
                var tables = new List<CaseStyleDb>();
                FileNames.ForEach(f => { tables.AddRange(Read<List<CaseStyleDb>>(f)); });
                DataList = tables;
            }

            private static T Read<T>(string sourceFileName) where T : class, new()
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