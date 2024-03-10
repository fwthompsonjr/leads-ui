using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc.entities
{
    public class PricingCodeBo
    {
        public string? Id { get; set; }
        public string? PermissionGroupId { get; set; }
        public string? KeyName { get; set; }
        public string? ProductCode { get; set; }
        public string? PriceCodeAnnual { get; set; }
        public string? PriceCodeMonthly { get; set; }
        public string? KeyJs { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateDate { get; set; }

        public ProductPricingModel? GetModel()
        {
            if (string.IsNullOrWhiteSpace(KeyJs)) return null;
            try
            {
                return JsonConvert.DeserializeObject<ProductPricingModel>(KeyJs);
            } 
            catch
            {
                return null;
            }
        }
    }
}
