using legallead.jdbc.entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IRefreshTokenValidator
    {
        UserRefreshToken? Verify(UserRefreshToken? token);
    }
}