﻿using legallead.harriscriminal.db.Downloads;
using System.Globalization;
using System.Reflection;

namespace legallead.harriscriminal.db.Entities
{
    public abstract class BaseDataLoad
    {
        private enum FileDateType
        {
            Max,
            Min
        }

        public abstract List<string> FileNames { get; set; }
        private static string? _appFolder;
        private static string? _dataFolder;
        private static string? _tableFolder;

        public static string AppFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_appFolder)) return _appFolder;
                _appFolder = GetAppFolderName();
                return _appFolder;
            }
        }

        public static string DataFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_dataFolder)) return _dataFolder;
                _dataFolder = GetDataFolderName();
                return _dataFolder;
            }
        }

        public static string TableFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_tableFolder)) return _tableFolder;
                _tableFolder = GetTableFolderName();
                return _tableFolder;
            }
        }

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppFolderName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            return Path.GetDirectoryName(execName) ?? string.Empty;
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

        /// <summary>
        /// Gets the name of the application tables directory.
        /// </summary>
        /// <returns></returns>
        private static string GetTableFolderName()
        {
            var parentName = AppFolder;
            return Path.Combine(parentName, "_db", "_tables");
        }

        public void Fetch<T>(IProgress<DataLoadDto> progress, DataLoadDto dto, List<T> tables)
            where T : class, new()
        {
            foreach (var f in FileNames)
            {
                dto.Processed = FileNames.IndexOf(f);
                progress.Report(dto);
                var item = Read<T>(f);
                dto.Processed += 1;
                tables.Add(item);
                progress.Report(dto);
            }
            dto.IsComplete = true;
            progress.Report(dto);
        }

        public void Map(IProgress<DataLoadDto> progress, DataLoadDto dto, List<HarrisCountyListDto> records)
        {
            var fileInfos = FileNames.Select(a => new FileInfo(a)).ToList();

            foreach (var item in fileInfos)
            {
                dto.Processed = fileInfos.IndexOf(item);
                progress.Report(dto);
                var data = HarrisCriminalDto.Map(item.FullName);
                var business = HarrisCriminalBo.Map(data);
                business.Sort((a, b) =>
                {
                    int aa = a.DateFiled.CompareTo(b.DateFiled);
                    if (aa != 0) return aa;
                    int bb = a.NextAppearanceDate.CompareTo(b.NextAppearanceDate);
                    return bb;
                });
                dto.Processed += 1;
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
                progress.Report(dto);
            }
            dto.IsComplete = true;
            progress.Report(dto);
        }

        protected static T Read<T>(string sourceFileName) where T : class, new()
        {
            var content = GetFileContent(sourceFileName);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content) ?? new();
        }

        private static string GetFileContent(string sourceFileName)
        {
            using var reader = new StreamReader(sourceFileName);
            return reader.ReadToEnd();
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
}