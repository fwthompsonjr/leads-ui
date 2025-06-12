using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class GetAdminStatusBo
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("isAdmin")] public bool? IsAdmin { get; set; }
    }
}