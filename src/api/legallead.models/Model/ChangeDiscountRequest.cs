namespace legallead.permissions.api.Model
{
    public class ChangeDiscountRequest
    {
        public IEnumerable<DiscountChoice> Choices { get; set; } = Array.Empty<DiscountChoice>();
    }
}