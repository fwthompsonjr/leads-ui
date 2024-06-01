namespace legallead.permissions.api.Entities
{
    public class DiscountModificationResponse : SubscriptionModificationResponse
    {
        public List<DiscountPaymentBo> Data { get; set; } = [];
    }
}
