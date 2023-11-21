using legallead.records.search.Classes;
using legallead.records.search.Models;
using OpenQA.Selenium;
using System.Drawing.Imaging;
using System.Text;

namespace legallead.records.search.Dto
{
    public class Court
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class CourtLocation
    {
        public string Id { get; set; } = string.Empty;

        public IList<Court> Courts { get; set; } = new List<Court>();
    }

    public class CourtLookup
    {
        public IList<CourtLocation> CourtLocations { get; set; } = new List<CourtLocation>();
    }

    public class Example
    {
        public SearchSettingDto SearchSetting { get; set; } = new();
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
            try
            {
                string dataFile = CourtLookupFile();
                string data = File.ReadAllText(dataFile);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<CourtLookup>(data) ?? new();
            }
            catch (Exception)
            {
                return GetAlternateCourtLookup();
            }
        }

        private static CourtLookup GetAlternateCourtLookup()
        {
            string data = FallbackJson.GetJs("courtAddress.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CourtLookup>(data) ?? new();
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


        private static class FallbackJson
        {
            public static string GetJs(string json)
            {
                const string tilde = "~";
                string quote = '"'.ToString();
                var sbb = new StringBuilder();
                switch (json)
                {
                    case "courtAddress.json":
                        sbb.AppendLine("{");
                        sbb.AppendLine("  ~courtLocations~: [");
                        sbb.AppendLine("    {");
                        sbb.AppendLine("      ~id~: ~1~,");
                        sbb.AppendLine("      ~courts~: [");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~default~,");
                        sbb.AppendLine("          ~address~: ~~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Court At Law #1~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 1~,");
                        sbb.AppendLine("          ~address~: ~210 South Woodrow Lane Denton, TX 76205-6304~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Court At Law #2~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 2~,");
                        sbb.AppendLine("          ~address~: ~1451 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #1~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #1~,");
                        sbb.AppendLine("          ~address~: ~1463 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #2~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #2~,");
                        sbb.AppendLine("          ~address~: ~1457 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #3~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #3~,");
                        sbb.AppendLine("          ~address~: ~1452 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #4~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #4~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #5~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #5~,");
                        sbb.AppendLine("          ~address~: ~1462 East McKinney Street, Denton TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Justice of the Peace Pct #6~,");
                        sbb.AppendLine("          ~fullName~: ~Justice of the Peace Pct #6~,");
                        sbb.AppendLine("          ~address~: ~1029 West Rosemeade Parkway, Carrollton, TX 75007-6251~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Criminal Court #1~,");
                        sbb.AppendLine("          ~fullName~: ~County Criminal Court #1~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Suite 1419, Denton, TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Criminal Court #2~,");
                        sbb.AppendLine("          ~fullName~: ~County Criminal Court #2~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Denton, TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Criminal Court #3~,");
                        sbb.AppendLine("          ~fullName~: ~County Criminal Court #3~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Suite 2447, Denton, TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Criminal Court #4~,");
                        sbb.AppendLine("          ~fullName~: ~County Criminal Court #4~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Suite 2306, Denton, TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Criminal Court #5~,");
                        sbb.AppendLine("          ~fullName~: ~County Criminal Court #5~,");
                        sbb.AppendLine("          ~address~: ~1450 East McKinney Street, Suite 2315, Denton, TX 76209-4524~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~Probate Court~,");
                        sbb.AppendLine("          ~fullName~: ~Probate Court~,");
                        sbb.AppendLine("          ~address~: ~1450 E McKinney St # 2412, Denton, TX 76209~");
                        sbb.AppendLine("        }");
                        sbb.AppendLine("      ]");
                        sbb.AppendLine("    },");
                        sbb.AppendLine("    {");
                        sbb.AppendLine("      ~id~: ~10~,");
                        sbb.AppendLine("      ~courts~: [");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~default~,");
                        sbb.AppendLine("          ~address~: ~100 West Weatherford, Fort Worth, TX 76196-0240~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Court at Law No. 1~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law No. 1~,");
                        sbb.AppendLine("          ~address~: ~100 West Weatherford, Fort Worth, TX 76196-0240~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Court at Law No. 2~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law No. 2~,");
                        sbb.AppendLine("          ~address~: ~100 West Weatherford, Fort Worth, TX 76196-0240~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~County Court at Law No. 3~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law No. 3~,");
                        sbb.AppendLine("          ~address~: ~100 West Weatherford, Fort Worth, TX 76196-0240~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 1~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 1~,");
                        sbb.AppendLine("          ~address~: ~100 West Weatherford, Fort Worth, TX 76196-0240~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 2~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 2~,");
                        sbb.AppendLine("          ~address~: ~701 East Abram Street, Suite 200, Arlington, Texas 76010~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 3~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 3~,");
                        sbb.AppendLine("          ~address~: ~Northeast Courthouse, 645 Grapevine Highway, Suite 220, Hurst, Texas 76054~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 4~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 4~,");
                        sbb.AppendLine("          ~address~: ~6713 Telephone Road, Suite 201, Lake Worth Texas 76135~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 5~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 5~,");
                        sbb.AppendLine("          ~address~: ~350 W. Belknap, Room 112-C, Fort Worth, Texas 76196~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 6~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 6~,");
                        sbb.AppendLine("          ~address~: ~6559 Granbury Road, Fort Worth, Texas 76133~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 7~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 7~,");
                        sbb.AppendLine("          ~address~: ~1105 E. Broad Street, Suite 202, Mansfield Texas 76063~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP No. 8~,");
                        sbb.AppendLine("          ~fullName~: ~JP No. 8~,");
                        sbb.AppendLine("          ~address~: ~Poly Subcourthouse, 3500 Miller Avenue, Fort Worth, Texas 76119~");
                        sbb.AppendLine("        }");
                        sbb.AppendLine("      ]");
                        sbb.AppendLine("    },");
                        sbb.AppendLine("    {");
                        sbb.AppendLine("      ~id~: ~20~,");
                        sbb.AppendLine("      ~courts~: [");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~default~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP 1~,");
                        sbb.AppendLine("          ~fullName~: ~Precinct 1~,");
                        sbb.AppendLine("          ~address~: ~2300 Bloomdale Rd., Suite 1164, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP 2~,");
                        sbb.AppendLine("          ~fullName~: ~Precinct 2~,");
                        sbb.AppendLine("          ~address~: ~406A Raymond Street, Farmersville, TX 75442~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP 3-1~,");
                        sbb.AppendLine("          ~fullName~: ~Precinct 3-1~,");
                        sbb.AppendLine("          ~address~: ~920 E. Park Blvd., Plano, TX 75074~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP 3-2~,");
                        sbb.AppendLine("          ~fullName~: ~Precinct 3-2~,");
                        sbb.AppendLine("          ~address~: ~920 E. Park Blvd., Plano, TX 75074~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~JP 4~,");
                        sbb.AppendLine("          ~fullName~: ~Precinct 4~,");
                        sbb.AppendLine("          ~address~: ~8585 John Wesley, Suite 130, Frisco, TX 75034~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 1~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 1~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 2~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 2~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 3~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 3~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 4~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 4~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 5~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 5~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 6~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 6~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~CCL 7~,");
                        sbb.AppendLine("          ~fullName~: ~County Court at Law 7~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~199th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~199th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~296th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~296th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~429th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~429th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~471st District Court~,");
                        sbb.AppendLine("          ~fullName~: ~471st District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~468th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~468th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~469th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~469th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~470th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~470th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~380th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~380th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~401st District Court~,");
                        sbb.AppendLine("          ~fullName~: ~401st District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~416th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~416th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~417th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~417th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~219th District Court~,");
                        sbb.AppendLine("          ~fullName~: ~219th District Court~,");
                        sbb.AppendLine("          ~address~: ~2100 Bloomdale Road, McKinney, TX 75071~");
                        sbb.AppendLine("        }");
                        sbb.AppendLine("      ]");
                        sbb.AppendLine("    },");
                        sbb.AppendLine("    {");
                        sbb.AppendLine("      ~id~: ~30~,");
                        sbb.AppendLine("      ~courts~: [");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~default~,");
                        sbb.AppendLine("          ~address~: ~201 Caroline -- Suite 740, Houston TX 77002-1900~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~1~,");
                        sbb.AppendLine("          ~fullName~: ~Harris County Civil Court at Law #1~,");
                        sbb.AppendLine("          ~address~: ~Harris County Civil Court at Law #1, 201 Caroline -- Suite 740, Houston TX 77002-1900~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~2~,");
                        sbb.AppendLine("          ~fullName~: ~Harris County Civil Court at Law #2~,");
                        sbb.AppendLine("          ~address~: ~Harris County Civil Court at Law #2, 201 Caroline -- Suite 740, Houston TX 77002-1900~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~3~,");
                        sbb.AppendLine("          ~fullName~: ~Harris County Civil Court at Law #3~,");
                        sbb.AppendLine("          ~address~: ~Harris County Civil Court at Law #3, 201 Caroline -- Suite 740, Houston TX 77002-1900~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~4~,");
                        sbb.AppendLine("          ~fullName~: ~Harris County Civil Court at Law #4~,");
                        sbb.AppendLine("          ~address~: ~Harris County Civil Court at Law #4, 201 Caroline -- Suite 740, Houston TX 77002-1900~");
                        sbb.AppendLine("        }");
                        sbb.AppendLine("      ]");
                        sbb.AppendLine("    },");
                        sbb.AppendLine("    {");
                        sbb.AppendLine("      ~id~: ~40~,");
                        sbb.AppendLine("      ~courts~: [");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~default~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court, 1201 Franklin St, Houston TX 77002-1900~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~001~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 1, 1201 Franklin St -- 8th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~002~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 2, 1201 Franklin St -- 8th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~003~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 3, 1201 Franklin St -- 8th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~004~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 4, 1201 Franklin St -- 8th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~005~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 5, 1201 Franklin St -- 9th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~006~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 6, 1201 Franklin St -- 9th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~007~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 7, 1201 Franklin St -- 9th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~008~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 8, 1201 Franklin St -- 9th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~009~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 9, 1201 Franklin St -- 10th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~010~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 10, 1201 Franklin St -- 10th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~011~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 11, 1201 Franklin St -- 10th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~012~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 12, 1201 Franklin St -- 10th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~013~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 13, 1201 Franklin St -- 11th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~014~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 14, 1201 Franklin St -- 11th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~015~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 15, 1201 Franklin St -- 11th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~016~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 16, 1201 Franklin St -- 11th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~174~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 174, 1201 Franklin St -- 19th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~176~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 176, 1201 Franklin St -- 19th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~177~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 177, 1201 Franklin St -- 19th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~178~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 178, 1201 Franklin St -- 19th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~179~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 179, 1201 Franklin St -- 18th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~180~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 180, 1201 Franklin St -- 18th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~182~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 182, 1201 Franklin St -- 18th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~183~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 183, 1201 Franklin St -- 18th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~184~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 184, 1201 Franklin St -- 17th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~185~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 185, 1201 Franklin St -- 17th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~208~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 208, 1201 Franklin St -- 17th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~209~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 209, 1201 Franklin St -- 17th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~228~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 228, 1201 Franklin St -- 16th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~230~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 230, 1201 Franklin St -- 16th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~232~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 232, 1201 Franklin St -- 16th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~248~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 248, 1201 Franklin St -- 16th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~262~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 262, 1201 Franklin St -- 15th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~263~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 263, 1201 Franklin St -- 15th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~337~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 337, 1201 Franklin St -- 15th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~338~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 338, 1201 Franklin St -- 15th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~339~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 339, 1201 Franklin St -- 14th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~351~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law No. 351, 1201 Franklin St -- 14th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~482~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law  No. 482, 1201 Franklin St -- 14th Floor, Houston TX 77002~");
                        sbb.AppendLine("        },");
                        sbb.AppendLine("        {");
                        sbb.AppendLine("          ~name~: ~RIC~,");
                        sbb.AppendLine("          ~address~: ~Harris County Criminal Court at Law (RIC), 1201 Franklin St -- 20th Floor, Houston TX 77002~");
                        sbb.AppendLine("        }");
                        sbb.AppendLine("      ]");
                        sbb.AppendLine("    }");
                        sbb.AppendLine("  ]");
                        sbb.AppendLine("}");
                        break;
                }
                sbb.Replace(tilde, quote);
                return sbb.ToString();

            }
        }

    }



}