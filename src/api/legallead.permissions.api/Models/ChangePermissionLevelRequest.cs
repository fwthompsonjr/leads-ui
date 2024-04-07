namespace legallead.permissions.api.Model
{
    public class ChangePermissionLevelRequest
    {
        public IEnumerable<PermissionChoice> Choices { get; set; } = Array.Empty<PermissionChoice>();
    }
}