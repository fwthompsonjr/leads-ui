using legallead.jdbc.entities;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api.Utility
{
    public class RequestedUserService : IRequestedUser
    {
        protected readonly IDataProvider _db;
        public RequestedUserService(IDataProvider db)
        {
            _db = db;
        }

        public async Task<User?> GetUser(HttpRequest request)
        {
            return await request.GetUser(_db);
        }
    }
}
