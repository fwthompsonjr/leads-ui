using legallead.permissions.api.Enumerations;

namespace legallead.permissions.api.Models
{
    public class TokenValidationModel
    {
        public bool Validated { get { return Errors.Count == 0; } }
        public List<TokenValidationStatus> Errors { get; } = [];
    }
}
