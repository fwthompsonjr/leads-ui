using System.ComponentModel.DataAnnotations;

namespace legallead.json.db.attr
{
    public class UsStateAttribute : ValidationAttribute
    {
        public string? Name { get; set; }
        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string stateCode) return false;
            return UsStatesList.Verify(stateCode);
        }
    }
}
