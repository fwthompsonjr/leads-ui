using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    public class RequestTypeAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "Address,Email,Name,Phone".Split(',').ToList();
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            if (string.IsNullOrWhiteSpace(name)) return true;
            return types.Exists(t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}