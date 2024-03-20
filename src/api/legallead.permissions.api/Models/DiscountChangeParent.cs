namespace legallead.permissions.api.Models
{
    public class DiscountChangeParent
    {
        public IEnumerable<DiscountChangeRequest> Choices { get; set; } = Array.Empty<DiscountChangeRequest>();
    }
}
