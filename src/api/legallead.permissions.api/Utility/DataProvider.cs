using legallead.jdbc.entities;
using legallead.jdbc.interfaces;

namespace legallead.permissions.api
{
    public class DataProvider
    {
        private readonly IComponentRepository _componentDb;
        private readonly IPermissionMapRepository _permissionDb;
        private readonly IProfileMapRepository _profileDb;
        private readonly IUserPermissionRepository _userPermissionDb;
        private readonly IUserProfileRepository _userProfileDb;
        private readonly IUserTokenRepository _userTokenDb;
        private readonly IUserPermissionViewRepository _userPermissionVw;
        private readonly IUserProfileViewRepository _userProfileVw;
        private readonly IPermissionGroupRepository _permissionGroupDb;
        private readonly IUserRepository _userDb;

        internal DataProvider(
            IComponentRepository component,
            IPermissionMapRepository permissionDb,
            IProfileMapRepository profileDb,
            IUserPermissionRepository userPermissionDb,
            IUserProfileRepository userProfileDb,
            IUserTokenRepository userTokenDb,
            IUserPermissionViewRepository userPermissionVw,
            IUserProfileViewRepository userProfileVw, 
            IPermissionGroupRepository permissionGroupDb,
            IUserRepository user)
        {
            _componentDb = component;
            _permissionDb = permissionDb;
            _profileDb = profileDb;
            _userPermissionDb = userPermissionDb;
            _userProfileDb = userProfileDb;
            _userTokenDb = userTokenDb;
            _userPermissionVw = userPermissionVw;
            _userProfileVw = userProfileVw;
            _permissionGroupDb = permissionGroupDb;
            _userDb = user;
        }

        internal IComponentRepository ComponentDb => _componentDb;
        internal IPermissionMapRepository PermissionDb => _permissionDb;
        internal IProfileMapRepository ProfileDb => _profileDb;
        internal IUserPermissionRepository UserPermissionDb => _userPermissionDb;
        internal IUserProfileRepository UserProfileDb => _userProfileDb;
        internal IUserTokenRepository UserTokenDb => _userTokenDb;
        internal IUserPermissionViewRepository UserPermissionVw => _userPermissionVw;
        internal IUserProfileViewRepository UserProfileVw => _userProfileVw;
        internal IPermissionGroupRepository PermissionGroupDb => _permissionGroupDb;
        internal IUserRepository UserDb => _userDb;

        public virtual async Task<bool> InitializeProfile(User user)
        {
            var profiles = await ProfileDb.GetAll();
            if (!profiles.Any()) { return true; }
            var current = profiles.Select(p => new UserProfile
            {
                Id = Guid.NewGuid().ToString("D"),
                ProfileMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            }).ToList();
            var existing = await UserProfileDb.GetAll(user);
            var additions = current.FindAll(x => !existing.Any(a => a.ProfileMapId == x.ProfileMapId));
            if (!additions.Any()) { return true; }
            additions.ForEach(async a => await UserProfileDb.Create(a));
            return true;
        }
        public virtual async Task<bool> InitializePermission(User user)
        {
            var permissions = await PermissionDb.GetAll();
            if (!permissions.Any()) { return true; }
            var current = permissions.Select(p => new UserPermission
            {
                Id = Guid.NewGuid().ToString("D"),
                PermissionMapId = p.Id ?? string.Empty,
                UserId = user.Id ?? string.Empty,
                KeyValue = string.Empty
            }).ToList();
            var existing = await UserPermissionDb.GetAll(user);
            var additions = current.FindAll(x => !existing.Any(a => a.PermissionMapId == x.PermissionMapId));
            if (!additions.Any()) { return true; }
            additions.ForEach(async a => await UserPermissionDb.Create(a));
            return true;
        }

        public virtual async Task<KeyValuePair<bool, string>> SetPermissionGroup(User user, string groupName)
        {
            try
            {
                var groups = ((await PermissionGroupDb.GetAll()) ?? Array.Empty<PermissionGroup>()).ToList();
                var permissions = ((await UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>()).ToList();
                if (!groups.Any()) return new KeyValuePair<bool, string>(false, "No groups defined in repository.");
                if (!permissions.Any()) return new KeyValuePair<bool, string>(false, "No permissions defined for user.");
                var group = groups.Find(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase) && g.IsActive.GetValueOrDefault());
                if (group == null) return new KeyValuePair<bool, string>(false, $"Group with name '{groupName} not defined.");
                var settings = new Dictionary<string, string>()
                {
                    { "Account.IsPrimary", "True" },
                    { "Account.Permission.Level", group.Name ?? string.Empty },
                    { "Setting.MaxRecords.Per.Year", group.PerYear.GetValueOrDefault().ToString() ?? string.Empty },
                    { "Setting.MaxRecords.Per.Month", group.PerMonth.GetValueOrDefault().ToString()},
                    { "Setting.MaxRecords.Per.Request", group.PerRequest.GetValueOrDefault().ToString() },
                };
                var changes = new List<UserPermission>();
                foreach (var item in settings.Keys)
                {
                    var permission = permissions.Find(p => p.KeyName.Equals(item, StringComparison.OrdinalIgnoreCase));
                    if (permission == null) return new KeyValuePair<bool, string>(false, $"Expected key: '{item}' not found in user settings");
                    changes.Add(new UserPermission
                    {
                        Id = permission.Id,
                        UserId = user.Id ?? Guid.Empty.ToString(),
                        PermissionMapId = permission.PermissionMapId,
                        KeyValue = settings[item]
                    });
                }
                changes.ForEach(async c =>
                {
                    await UserPermissionDb.Update(c);
                });
                return new KeyValuePair<bool, string>(true, $"Group seetings for '{groupName} applied to user account.");
            }
            catch (Exception)
            {
                return new KeyValuePair<bool, string>(false, $"Unexpected error occurred appling user settings.");
            }
        }
    }
}