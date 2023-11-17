using legallead.records.search.Classes;
using legallead.records.search.Models;

namespace legallead.records.search.Dto
{
    public class Court
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }

    public class CourtLocation
    {
        public string Id { get; set; }

        public IList<Court> Courts { get; set; }
    }

    public class CourtLookup
    {
        public IList<CourtLocation> CourtLocations { get; set; }
    }

    public class Example
    {
        public SearchSettingDto SearchSetting { get; set; }
    }

    public class SearchSettingDto
    {
        public int CountySearchTypeId { get; set; }
        public int CountyCourtId { get; set; }
        public int DistrictCourtId { get; set; }
        public int DistrictSearchTypeId { get; set; }

        public static CourtLookup GetCourtLookupList
        {
            get { return _courtJson ??= GetCourtLookup(); }
        }

        private static CourtLookup? _courtJson;

        private static CourtLookup GetCourtLookup()
        {
            string dataFile = CourtLookupFile();
            string data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CourtLookup>(data);
        }

        public static NavInstruction GetNonCriminalMapping()
        {
            string dataFile = NonCriminalMappingFile();
            string data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavInstruction>(data) ?? new();
        }

        public static NavInstruction GetCriminalMapping()
        {
            string dataFile = CriminalMappingFile();
            string data = File.ReadAllText(dataFile);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<NavInstruction>(data) ?? new();
        }

        public static SearchSettingDto GetDto()
        {
            string dataFile = DataFile();
            string data = File.ReadAllText(dataFile);
            Example? parent = Newtonsoft.Json.JsonConvert.DeserializeObject<Example>(data) ?? new();
            return parent.SearchSetting;
        }

        public static void Save(SearchSettingDto source)
        {
            string dataFile = DataFile();
            Example parent = new() { SearchSetting = source };
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(parent, Newtonsoft.Json.Formatting.Indented);
            if (File.Exists(dataFile)) { File.Delete(dataFile); }
            using StreamWriter sw = new(dataFile);
            sw.Write(data);
        }

        private static string DataFile()
        {
            const string fileSuffix = "denton-settings";
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string CriminalMappingFile()
        {
            const string fileSuffix = "dentonCaseCustomInstruction";
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string NonCriminalMappingFile()
        {
            const string fileSuffix = "dentonCaseCustomInstruction_1";
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        private static string CourtLookupFile()
        {
            const string fileSuffix = "courtAddress";
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(SearchSettingFileNotFound);
            }
            return dataFile;
        }

        internal static readonly string SearchSettingFileNotFound
            = CommonKeyIndexes.SearchSettingFileNotFound;
    }
}