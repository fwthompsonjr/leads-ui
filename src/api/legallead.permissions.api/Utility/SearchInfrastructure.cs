using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Model;
using Newtonsoft.Json;

namespace legallead.permissions.api.Utility
{
    public class SearchInfrastructure : ISearchInfrastructure
    {
        protected readonly IDataProvider _db;
        private readonly IUserSearchRepository _repo;
        internal SearchInfrastructure(IDataProvider db, IUserSearchRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        public async Task<UserSearchBeginResponse?> Begin(HttpRequest http, UserSearchRequest request)
        {
            var user = await GetUser(http);
            if (user == null) return null;
            var searchRecord = await _repo.Begin(user.Id, JsonConvert.SerializeObject(request));
            return new()
            {
                Request = request,
                RequestId = searchRecord.Key ? searchRecord.Value : string.Empty
            };
        }

        public async Task<IEnumerable<UserSearchHeader>?> GetHeader(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
            if (user == null) return null;
            return Enumerable.Empty<UserSearchHeader>();
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetDetail(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
            if (user == null) return null;
            return new[]
            {
                new UserSearchDetail
                {
                    Context = "Detail"
                }
            };
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetResult(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
            if (user == null)
                return null;
            return new[]
            {
                new UserSearchDetail
                {
                    Context = "Result"
                }
            };
        }

        public async Task<IEnumerable<UserSearchDetail>?> GetStatus(HttpRequest http, string? id)
        {
            var user = await GetUser(http);
            if (user == null)
                return null;
            return new[]
            {
                new UserSearchDetail
                {
                    Context = "Status"
                }
            };
        }

        public Task<bool> SetDetail(string id, object detail)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetResult(string id, object result)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetStatus(string id, string message)
        {
            return Task.FromResult(true);
        }
        public Task<bool> SetResponse(string id, object response)
        {
            return Task.FromResult(true);
        }

        public async Task<User?> GetUser(HttpRequest request)
        {
            return await request.GetUser(_db);
        }
    }
}
