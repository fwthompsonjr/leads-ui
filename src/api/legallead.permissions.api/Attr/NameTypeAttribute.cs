using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    public class NameTypeAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "First,Last,Company".Split(',').ToList();
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            return types.Exists(t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}