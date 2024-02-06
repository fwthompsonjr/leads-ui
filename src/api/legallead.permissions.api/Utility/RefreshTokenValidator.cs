using legallead.jdbc.entities;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api
{
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
        private const int defaultExpiryHours = 8;
        private readonly int? _expiryHours;

        public RefreshTokenValidator(IConfiguration iconfiguration)
        {
            var hours = iconfiguration.GetValue<int>("RefreshWindow");
            if (hours == 0) hours = defaultExpiryHours;
            _expiryHours = hours;
        }

        public UserRefreshToken? Verify(UserRefreshToken? token)
        {
            if (token == null) return null;
            if (token.CreateDate == null) return null;
            var tspan = TimeSpan.FromHours(_expiryHours.GetValueOrDefault(defaultExpiryHours));
            var creationExpiry = token.CreateDate.Value.Add(tspan);
            var now = DateTime.UtcNow;
            token.IsActive = now < creationExpiry;
            return token;
        }
    }
}