using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    public class PhoneTypeAttribute : ValidationAttribute
    {
        protected static readonly List<string> types = "Personal,Business,Other".Split(',').ToList();

        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string phoneType) return false;
            return types.Exists(t => t.Equals(phoneType, StringComparison.OrdinalIgnoreCase));
        }
    }
}