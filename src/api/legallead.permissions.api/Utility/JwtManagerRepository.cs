using legallead.jdbc.entities;
using legallead.permissions.api.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace legallead.permissions.api
{
    public class JwtManagerRepository : IJwtManagerRepository
    {
        private const int defaultExpiryMinutes = 5;
        private readonly IConfiguration _iconfiguration;
        private readonly int? _expiryMinutes;

        public JwtManagerRepository(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            var minutes = iconfiguration.GetValue<int>("AuthorizationWindow");
            if (minutes == 0) minutes = defaultExpiryMinutes;
            _expiryMinutes = minutes;
        }
        public Tokens? GenerateToken(User user)
        {
            return GenerateJWTTokens(user);
        }

        public Tokens? GenerateJWTTokens(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyconfig = _iconfiguration["JWT:Key"] ?? string.Empty;
                var tokenKey = Encoding.UTF8.GetBytes(keyconfig);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                  {
                      new Claim(ClaimTypes.Name, user.Email),
                      new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                      new Claim(ClaimTypes.WindowsAccountName, user.UserName),
                      new Claim(ClaimTypes.Email, user.Email)
                  }),
                    Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes.GetValueOrDefault(defaultExpiryMinutes)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                return new Tokens { AccessToken = tokenHandler.WriteToken(token), RefreshToken = refreshToken };
            }
            catch
            {
                return null;
            }
        }


        public Tokens? GenerateRefreshToken(User user)
        {
            return GenerateJWTTokens(user);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = GetValidationParameters();

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenValidationParameters = GetValidationParameters();
                var tokenHandler = new JwtSecurityTokenHandler();
                _ = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken.ValidTo < DateTime.UtcNow) { return false; }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private TokenValidationParameters GetValidationParameters()
        {
            var keyconfig = _iconfiguration["JWT:Key"] ?? string.Empty;
            var Key = Encoding.UTF8.GetBytes(keyconfig);
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
