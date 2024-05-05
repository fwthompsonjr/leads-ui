namespace legallead.permissions.api.Interfaces
{
    public interface IUserSearchValidator
    {
        KeyValuePair<bool, string> IsValid(UserSearchRequest request);
    }
}
