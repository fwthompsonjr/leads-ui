using legallead.permissions.api.Model;

namespace legallead.permissions.api.Models
{
    public class DiscountChangeRequest : DiscountChoice
    {
        public string? MonthlyBillingCode { get; set; }
        public string? AnnualBillingCode { get; set; }
    }
}
