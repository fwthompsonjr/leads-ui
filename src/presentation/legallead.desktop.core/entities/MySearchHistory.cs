using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    public class MySearchHistory
    {
        [JsonProperty("searches")]
        public List<MySearchItem> Searches { get; set; } = new();

        [JsonProperty("statuses")]
        public List<MySearchStatus> Statuses { get; set; } = new();

        [JsonProperty("staged")]
        public List<MySearchStaged> Staged { get; set; } = new();
    }
}
