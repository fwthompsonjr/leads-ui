using legallead.permissions.api.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidUserManagementMethodAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string methodName)
            {
                return Enum.IsDefined(typeof(UserManagementMethod), methodName);
            }
            return false;
        }
    }
}
