using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IProfileRequestVerification
    {
        Task<ActionUserResponse> VerifyRequest(HttpRequest http, object[] request);
    }
}
