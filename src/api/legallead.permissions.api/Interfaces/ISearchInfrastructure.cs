using legallead.jdbc.entities;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ISearchInfrastructure
    {
        Task<User?> GetUser(HttpRequest request);

        Task<UserSearchBeginResponse?> Begin(HttpRequest http, UserSearchRequest request);

        Task<IEnumerable<UserSearchDetail>?> GetStatus(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetDetail(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchDetail>?> GetResult(HttpRequest http, string? id);
        Task<IEnumerable<UserSearchQueryModel>?> GetHeader(HttpRequest http, string? id);

        Task<bool> SetStatus(string id, string message);

        Task<bool> SetDetail(string id, object detail);

        Task<bool> SetResult(string id, object result);
        Task<bool> SetResponse(string id, object response);
    }
}
