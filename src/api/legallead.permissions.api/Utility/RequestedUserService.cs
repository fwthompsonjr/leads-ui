using legallead.jdbc.entities;
using legallead.permissions.api.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Utility
{
    public class RequestedUserService : IRequestedUser
    {
        protected readonly IDataProvider _db;
        public RequestedUserService(IDataProvider db)
        {
            _db = db;
        }
        [ExcludeFromCodeCoverage(Justification = "This wrapper method is tested elswwhere.")]
        public async Task<User?> GetUser(HttpRequest request)
        {
            return await request.GetUser(_db);
        }
    }
}
