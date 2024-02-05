using legallead.jdbc.entities;
using legallead.permissions.api.Model;
using System.Security.Claims;

namespace legallead.permissions.api.Interfaces
{
    public interface IJwtManagerRepository
    {
        Tokens? GenerateToken(User user);

        Tokens? GenerateRefreshToken(User user);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        bool ValidateToken(string token);
    }
}