using legallead.jdbc.entities;
using legallead.json.db.entity;

namespace legallead.permissions.api.Interfaces
{
    public interface ISubscriptionInfrastructure
    {
        Task<bool> IsAdminUser(HttpRequest request);

        Task<User?> GetUser(HttpRequest request);

        UsState? FindState(string? state);

        List<UsStateCounty>? FindAllCounties(string? county);

        Task<KeyValuePair<bool, string>> AddCountySubscriptions(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> AddStateSubscriptions(User user, string stateCode);

        Task<KeyValuePair<bool, string>> RemoveCountySubscriptions(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> RemoveStateSubscriptions(User user, string stateCode);

        Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName);
        Task<LevelRequestBo> GeneratePermissionSession(HttpRequest request, User user, string level, string externalId = "");
    }
}