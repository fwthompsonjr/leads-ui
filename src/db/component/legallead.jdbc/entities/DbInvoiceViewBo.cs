using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class DbInvoiceViewBo
    {
        [JsonProperty("id")] public string? Id { get; set; }
        [JsonProperty("userName")] public string? UserName { get; set; }
        [JsonProperty("email")] public string? Email { get; set; }
        [JsonProperty("userId")] public string? LeadUserId { get; set; }
        [JsonProperty("rqid")] public string? RequestId { get; set; }
        [JsonProperty("invoiceno")] public string? InvoiceNbr { get; set; }
        [JsonProperty("uri")] public string? InvoiceUri { get; set; }
        [JsonProperty("rc")] public int? RecordCount { get; set; }
        [JsonProperty("linenbr")] public int? LineNbr { get; set; }
        [JsonProperty("desc")] public string? Description { get; set; }
        [JsonProperty("itemCount")] public int? ItemCount { get; set; }
        [JsonProperty("itemPrice")] public decimal? ItemPrice { get; set; }
        [JsonProperty("itemTotal")] public decimal? ItemTotal { get; set; }
        [JsonProperty("total")] public decimal? InvoiceTotal { get; set; }
        [JsonProperty("completeDt")] public DateTime? CompleteDate { get; set; }
        [JsonProperty("createDt")] public DateTime? CreateDate { get; set; }
    }
}