using Newtonsoft.Json;

namespace legallead.email.models
{


    public class UserRecordSearch
    {
        [JsonProperty("requestId")]
        public string SearchRequestId { get; set; } = string.Empty;

        [JsonProperty("request")]
        public SearchRequest Search { get; set; } = new();
    }

    public class CountyDetail
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class SearchRequestItem
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class SearchRequest
    {
        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        [JsonProperty("county")]
        public CountyDetail County { get; set; } = new();

        [JsonProperty("details")]
        public List<SearchRequestItem> Details { get; set; } = [];

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }

}
