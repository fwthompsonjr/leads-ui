using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GetInvoiceTypeAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "Customer,Invoice".Split(',').ToList();
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string address) return false;
            return types.Exists(t => t.Equals(address, StringComparison.OrdinalIgnoreCase));
        }
    }
}