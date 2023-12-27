using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    public class AddressTypeAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "Mailing,Billing".Split(',').ToList();
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string address) return false;
            return types.Exists(t => t.Equals(address, StringComparison.OrdinalIgnoreCase));
        }
    }
}