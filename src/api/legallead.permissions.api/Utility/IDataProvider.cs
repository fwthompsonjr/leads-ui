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

        Task<KeyValuePair<bool, string>> AddCountySubscriptions(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> AddStateSubscriptions(User user, string stateCode);

        Task<bool> InitializePermission(User user);

        Task<bool> InitializeProfile(User user);

        Task<KeyValuePair<bool, string>> RemoveCountySubscriptions(User user, UsStateCounty countyCode);

        Task<KeyValuePair<bool, string>> RemoveStateSubscriptions(User user, string stateCode);

        Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName);
    }
}