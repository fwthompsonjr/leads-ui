namespace legallead.jdbc.entities
{
    public class UserPermission
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string PermissionMapId { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty;
    }
}