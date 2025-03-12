using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string index) return false;
            if (string.IsNullOrEmpty(index)) return true;
            return Guid.TryParse(index, out var _);
        }
    }
}