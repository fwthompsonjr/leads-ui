using legallead.jdbc.attr;

namespace legallead.permissions.api.Model
{
    public class UserLevelRequest
    {
        [PermissionName]
        public string Level { get; set; } = string.Empty;
    }
}