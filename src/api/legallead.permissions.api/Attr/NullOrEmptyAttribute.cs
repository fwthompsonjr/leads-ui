using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NullOrEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string str) return false;
            return string.IsNullOrEmpty(str);
        }
    }
}
