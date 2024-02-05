namespace legallead.models
{
    public class ChangeDiscountRequest
    {
        public IEnumerable<DiscountChoice> Choices { get; set; } = Array.Empty<DiscountChoice>();
    }
}