using legallead.jdbc.entities;
using legallead.permissions.api.Model;

namespace legallead.permissions.api.Utility
{
    public class ProfileInfrastructure : SubscriptionInfrastructure, IProfileInfrastructure
    {
        public ProfileInfrastructure(IDataProvider db) : base(db)
        {
        }

        public Task<KeyValuePair<bool, string>> ChangeContactAddress(User? user, ChangeContactAddressRequest[] request)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<bool, string>> ChangeContactEmail(User? user, ChangeContactEmailRequest[] request)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<bool, string>> ChangeContactName(User? user, ChangeContactNameRequest[] request)
        {
            throw new NotImplementedException();
        }

        public Task<KeyValuePair<bool, string>> ChangeContactPhone(User? user, ChangeContactPhoneRequest[] request)
        {
            throw new NotImplementedException();
        }
    }
}
