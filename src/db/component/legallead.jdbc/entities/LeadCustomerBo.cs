using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class LeadCustomerBo
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("lidx")] public string? LeadUserId { get; set; }
        [JsonProperty("cidx")] public string? CustomerId { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("isTest")] public bool? IsTest { get; set; }
        [JsonProperty("createDt")] public DateTime? CreateDate { get; set; }
    }
}
