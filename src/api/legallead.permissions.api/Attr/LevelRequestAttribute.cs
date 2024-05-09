using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Attr
{
    public class LevelRequestAttribute : ValidationAttribute
    {
        private static readonly List<string> types = "Admin,Platinum,Gold,Silver,Guest".Split(',').ToList();
        public string? Level { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            if (value is not string name) return false;
            if (string.IsNullOrWhiteSpace(name)) return true;
            return types.Exists(t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}