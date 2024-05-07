namespace legallead.email.models
{
    public class PermissionChangeResponse
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public PermissionLevelResponseBo Dto { get; set; } = new();
    }
}
