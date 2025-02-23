using System.ComponentModel.DataAnnotations;

namespace legallead.jdbc.attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CountyFileStatusAttribute : ValidationAttribute
    {
        private static readonly List<string> _fileStatuses =
        [
            "EMPTY",
            "ENCODED",
            "DECODED",
            "DOWNLOADED"
        ];

        public string Name { get; set; } = string.Empty;

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string permission) return false;
            return _fileStatuses.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}