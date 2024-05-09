namespace legallead.permissions.api.Interfaces
{
    public interface IRequestedUser
    {
        Task<User?> GetUser(HttpRequest request);
    }
}
