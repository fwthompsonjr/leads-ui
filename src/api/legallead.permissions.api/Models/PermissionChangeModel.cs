namespace legallead.permissions.api.Models
{
    public class PermissionChangeModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public LevelRequestBo Dto { get; set; } = new();
    }
}
