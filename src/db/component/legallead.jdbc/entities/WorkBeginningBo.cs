using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class WorkBeginningBo
    {
        [JsonProperty("src")]
        public string Source { get; set; } = Environment.MachineName;
        [JsonProperty("ids")]
        public IEnumerable<WorkIndexBo> WorkIndexes { get; set; } = Enumerable.Empty<WorkIndexBo>();
    }
}
