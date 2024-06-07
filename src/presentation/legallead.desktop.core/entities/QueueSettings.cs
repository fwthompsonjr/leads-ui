using legallead.desktop.interfaces;
using Newtonsoft.Json;

namespace legallead.desktop.entities
{
    public class QueueSettings : IQueueSettings
    {
        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        public string? FolderName { get; set; }
    }
}
