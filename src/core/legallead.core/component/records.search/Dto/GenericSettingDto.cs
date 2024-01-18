using legallead.records.search.Classes;
using System.Text;

namespace legallead.records.search.Dto
{
    public class GenericSettingDto : SearchSettingDto
    {
        private static Dictionary<string, GenericSetting>? _settings;

        public GenericSettingDto(string name)
        {
            _settings ??= new Dictionary<string, GenericSetting>();
            if (!_settings.ContainsKey(name))
            {
                _settings.Add(name, new GenericSetting { Name = name });
            }
            Name = name;
        }

        public string Name { get; private set; }

        public new void Save(SearchSettingDto source)
        {
            if (_settings == null) return;
            _settings[Name].Save(source);
        }

        public new SearchSettingDto GetDto()
        {
            if (_settings == null) return new();
            return _settings[Name].GetDto();
        }
    }

    public class GenericSetting
    {
        private string? _name;

        public string Name
        {
            get { return _name ?? string.Empty; }
            set
            {
                _name = value;
                string fileSuffix = value;
                const string dataFormat = @"{0}\xml\{1}.json";
                string appDirectory = ContextManagment.AppDirectory;
                string dataFile = string.Format(
                    CultureInfo.CurrentCulture,
                    dataFormat,
                    appDirectory,
                    fileSuffix);
                var fallback = Fallback(fileSuffix);
                DataFile = dataFile;
                Content = File.Exists(dataFile) ?
                    File.ReadAllText(dataFile) : fallback;
            }
        }

        public string DataFile { get; private set; } = string.Empty;

        public string Content { get; private set; } = string.Empty;

        public SearchSettingDto GetDto()
        {
            Example parent = Newtonsoft.Json.JsonConvert.DeserializeObject<Example>(Content) ?? new();
            return parent.SearchSetting;
        }

        public void Save(SearchSettingDto source)
        {
            string dataFile = DataFile;
            Example parent = new() { SearchSetting = source };
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(parent,
                Newtonsoft.Json.Formatting.Indented);
            if (File.Exists(dataFile)) { File.Delete(dataFile); }
            using (StreamWriter sw = new(dataFile))
            {
                sw.Write(data);
            }
            Content = File.ReadAllText(dataFile);
        }

        private static string Fallback(string input)
        {
            var sbb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // harris-civil-settings.json
            sbb.AppendLine("{");
            sbb.AppendLine("  ~SearchSetting~: {");
            sbb.AppendLine("    ~CountySearchTypeId~: 0,");
            sbb.AppendLine("    ~CountyCourtId~: 0,");
            sbb.AppendLine("    ~DistrictCourtId~: 0,");
            sbb.AppendLine("    ~DistrictSearchTypeId~: 0");
            sbb.AppendLine("  }");
            sbb.AppendLine("}");
            sbb.Replace('~', '"');
            return sbb.ToString();
        }
    }
}