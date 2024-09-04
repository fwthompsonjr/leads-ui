using legallead.json.db.entity;

namespace legallead.permissions.api.Interfaces
{
    public interface ISubscriptionInfrastructure
    {
        Task<bool> IsAdminUserAsync(HttpRequest request);

        Task<User?> GetUserAsync(HttpRequest request);

        UsState? FindState(string? state);

        List<UsStateCounty>? FindAllCounties(string? county);

        Task<KeyValuePair<bool, string>> AddCountySubscriptionsAsync(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> AddStateSubscriptionsAsync(User user, string stateCode);

        Task<KeyValuePair<bool, string>> RemoveCountySubscriptionsAsync(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> RemoveStateSubscriptionsAsync(User user, string stateCode);

        Task<KeyValuePair<bool, string>> SetPermissionGroupAsync(User user, string groupName);
        Task<LevelRequestBo> GeneratePermissionSessionAsync(HttpRequest request, User user, string level, string externalId = "");
        Task<LevelRequestBo?> GetLevelRequestByIdAsync(string? id, string? sessionid);
        Task<LevelRequestBo> GenerateDiscountSessionAsync(HttpRequest request, User user, string json, bool isAdmin, string externalId = "");
        Task<LevelRequestBo?> GetDiscountRequestByIdAsync(string? id, string? sessionid);
    }
}