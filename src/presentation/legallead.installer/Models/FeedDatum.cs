using Newtonsoft.Json;

namespace legallead.installer.Models
{
    public class FeedDatum
    {
        [JsonProperty("@id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("@type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("registration")]
        public string Registration { get; set; } = string.Empty;

        [JsonProperty("id")]
        public string Index { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; } = string.Empty;

        [JsonProperty("licenseUrl")]
        public string LicenseUrl { get; set; } = string.Empty;

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; set; } = string.Empty;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = [];

        [JsonProperty("authors")]
        public List<string> Authors { get; set; } = [];

        [JsonProperty("totaldownloads")]
        public int Totaldownloads { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;

        [JsonProperty("versions")]
        public List<FeedVersion> Versions { get; set; } = [];
    }
}
