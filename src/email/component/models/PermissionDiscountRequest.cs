namespace legallead.email.models
{
    public class PermissionDiscountRequest
    {
        public IEnumerable<PermissionDiscountChoice> Choices { get; set; } = Array.Empty<PermissionDiscountChoice>();
    }
}
