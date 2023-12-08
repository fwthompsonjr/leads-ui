using legallead.jdbc.entities;

namespace legallead.permissions.api
{
    public interface IRefreshTokenValidator
    {
        UserRefreshToken? Verify(UserRefreshToken? token);
    }
}