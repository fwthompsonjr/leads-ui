using System.ComponentModel.DataAnnotations;

namespace legallead.jdbc.attr
{
    public class PermissionNameAttribute : ValidationAttribute
    {
        private static readonly List<string> _permissionNames = new()
        {
            "None",
            "Guest",
            "Silver",
            "Gold",
            "Platinum",
            "Admin"
        };

        public string Name { get; set; } = string.Empty;

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string permission) return false;
            return _permissionNames.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}