namespace legallead.permissions.api.Interfaces
{
    public interface IRequestedUser
    {
        Task<User?> GetUserAsync(HttpRequest request);
    }
}
