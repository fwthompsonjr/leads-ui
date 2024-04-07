using legallead.models;

namespace legallead.permissions.api.Models
{
    public class DiscountChangeRequest : DiscountChoice
    {
        public string? MonthlyBillingCode { get; set; }
        public string? AnnualBillingCode { get; set; }
    }
}
