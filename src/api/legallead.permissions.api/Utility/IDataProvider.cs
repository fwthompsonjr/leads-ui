using legallead.jdbc.interfaces;
using legallead.json.db.entity;

namespace legallead.permissions.api
{
    public interface IDataProvider
    {
        IUserPermissionHistoryRepository PermissionHistoryDb { get; }
        IUserProfileHistoryRepository ProfileHistoryDb { get; }
        IUserProfileRepository UserProfileDb { get; }
        IUserProfileViewRepository UserProfileVw { get; }
        IUserPermissionViewRepository UserPermissionVw { get; }
        IUserRepository UserDb { get; }
        IComponentRepository ComponentDb { get; }
        IPermissionMapRepository PermissionDb { get; }
        IProfileMapRepository ProfileDb { get; }
        IUserTokenRepository UserTokenDb { get; }
        IPermissionGroupRepository PermissionGroupDb { get; }
        IUserPermissionRepository UserPermissionDb { get; }

        Task<KeyValuePair<bool, string>> AddCountySubscriptionsAsync(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> AddStateSubscriptionsAsync(User user, string stateCode);

        Task<bool> InitializePermissionAsync(User user);

        Task<bool> InitializeProfileAsync(User user);

        Task<KeyValuePair<bool, string>> RemoveCountySubscriptionsAsync(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> RemoveStateSubscriptionsAsync(User user, string stateCode);

        Task<KeyValuePair<bool, string>> SetPermissionGroupAsync(User user, string groupName);
    }
}