using Newtonsoft.Json;

namespace legallead.permissions.api.Models
{
    public class InvoiceDetailModel
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("linenbr")] public int? LineNbr { get; set; }
        [JsonProperty("desc")] public string? Description { get; set; }
        [JsonProperty("itemCount")] public int? ItemCount { get; set; }
        [JsonProperty("itemPrice")] public decimal? ItemPrice { get; set; }
        [JsonProperty("itemTotal")] public decimal? ItemTotal { get; set; }
    }
}
