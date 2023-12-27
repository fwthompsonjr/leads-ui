using legallead.jdbc.entities;
using legallead.permissions.api.Model;

namespace legallead.permissions.api
{
    public interface IProfileInfrastructure : ISubscriptionInfrastructure
    {
        Task<KeyValuePair<bool, string>> ChangeContactAddress(User? user, ChangeContactAddressRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactEmail(User? user, ChangeContactEmailRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactName(User? user, ChangeContactNameRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactPhone(User? user, ChangeContactPhoneRequest[] request);
    }
}