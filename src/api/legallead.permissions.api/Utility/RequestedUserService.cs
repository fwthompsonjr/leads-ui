namespace legallead.permissions.api.Utility
{
    public class RequestedUserService(IDataProvider db) : IRequestedUser
    {
        protected readonly IDataProvider _db = db;

        [ExcludeFromCodeCoverage(Justification = "This wrapper method is tested elswwhere.")]
        public async Task<User?> GetUserAsync(HttpRequest request)
        {
            return await request.GetUserAsync(_db);
        }
    }
}
