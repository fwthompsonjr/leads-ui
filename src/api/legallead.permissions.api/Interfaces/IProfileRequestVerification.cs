using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IProfileRequestVerification
    {
        Task<ActionUserResponse> VerifyRequestAsync(HttpRequest http, object[] request);
    }
}
