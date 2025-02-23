using System.ComponentModel.DataAnnotations;

namespace legallead.jdbc.attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CountyFileTypeAttribute : ValidationAttribute
    {
        private static readonly List<string> _fileTypes = new()
        {
            "NONE",
            "EXL",
            "CSV",
            "JSON"
        };

        public string Name { get; set; } = string.Empty;

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string permission) return false;
            return _fileTypes.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}