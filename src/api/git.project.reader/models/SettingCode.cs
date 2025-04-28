using Newtonsoft.Json;

namespace git.project.reader.models
{
    public class SettingCode
    {
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;
        [JsonProperty("vector")] public string Vector { get; set; } = string.Empty;
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name)) { return false; }
            if (string.IsNullOrWhiteSpace(Vector)) { return false; }
            return true;
        }
    }
}
