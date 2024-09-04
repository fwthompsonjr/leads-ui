namespace legallead.permissions.api.Interfaces
{
    public interface IProfileInfrastructure : ISubscriptionInfrastructure
    {
        Task<KeyValuePair<bool, string>> ChangeContactAddressAsync(User? user, ChangeContactAddressRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactEmailAsync(User? user, ChangeContactEmailRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactNameAsync(User? user, ChangeContactNameRequest[] request);

        Task<KeyValuePair<bool, string>> ChangeContactPhoneAsync(User? user, ChangeContactPhoneRequest[] request);

        Task<GetContactResponse[]?> GetContactDetailAsync(User? user, string responseType);

        Task<string> GetContactRoleAsync(User? user);
    }
}